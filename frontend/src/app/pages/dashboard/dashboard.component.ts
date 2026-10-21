import { Component, OnInit } from '@angular/core';
import { catchError, forkJoin, of } from 'rxjs';

import {
  AnalyticsPointDto,
  AnalyticsSeriesResponseDto,
  DashboardOverviewDto,
  KpiViewModel,
  TableColumn
} from '../../core/models/analytics.models';

import { AnalyticsApiService } from '../../core/services/analytics-api.service';
import { FormatService } from '../../core/services/format.service';
import { ChartDatasetInput } from '../../shared/components/chart-panel/chart-panel.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  loading = true;
  errorMessage = '';

  overview?: DashboardOverviewDto;

  kpiCards: KpiViewModel[] = [];

  trendLabels: string[] = [];
  trendDatasets: ChartDatasetInput[] = [];

  productLabels: string[] = [];
  productDatasets: ChartDatasetInput[] = [];

  cityLabels: string[] = [];
  cityDatasets: ChartDatasetInput[] = [];

  employeeLabels: string[] = [];
  employeeDatasets: ChartDatasetInput[] = [];

  customerRows: Array<Record<string, unknown>> = [];

  tableColumns: TableColumn[] = [
    { key: 'rank', label: '#', type: 'number' },
    { key: 'label', label: 'Customer', type: 'text' },
    { key: 'totalAmount', label: 'Total Amount', type: 'money' },
    // { key: 'orders', label: 'Orders', type: 'number' },
    // { key: 'averageBasket', label: 'Panier moyen', type: 'money' }
  ];

  constructor(
    private readonly api: AnalyticsApiService,
    private readonly format: FormatService
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    // Display partial results without hiding failed endpoints from the user.
    const failedSections: string[] = [];
    forkJoin({
      overview: this.api.getDashboardOverview({}).pipe(catchError(() => { failedSections.push('KPIs'); return of(null); })),
      trend: this.api.getSalesTrend(3, {}).pipe(catchError(() => { failedSections.push('sales trend'); return of(this.emptySeries('Sales Trend', 'Date', 'Month')); })),
      products: this.api.getProductPerformance(2, 0, 8, {}).pipe(catchError(() => { failedSections.push('products'); return of(this.emptySeries('Product Analytics', 'Product', 'Product')); })),
      cities: this.api.getGeographyPerformance(1, 0, 8, {}).pipe(catchError(() => { failedSections.push('cities'); return of(this.emptySeries('Geography Analytics', 'Geography', 'City')); })),
      employees: this.api.getEmployeePerformance(2, 0, 8, {}).pipe(catchError(() => { failedSections.push('employees'); return of(this.emptySeries('Employee Analytics', 'Employee', 'Employee')); }))
    }).subscribe({
      next: result => {
        if (result.overview) {
          this.overview = result.overview;
          this.kpiCards = this.toKpiCards(result.overview);
          this.customerRows = this.toCustomerRows(result.overview.topCustomers ?? []);
        } else {
          this.overview = undefined;
          this.kpiCards = [];
          this.customerRows = [];
        }

        this.buildTrendChart(result.trend);
        this.buildProductChart(result.products);
        this.buildCityChart(result.cities);
        this.buildEmployeeChart(result.employees);

        // Fallback: if a simple endpoint returns empty but dashboard/overview contains top arrays, use them.
        if (this.productLabels.length === 0 && result.overview?.topProducts?.length) {
          this.buildProductChart(this.seriesFromPoints('Product Analytics', 'Product', 'Product', result.overview.topProducts));
        }
        // Country-level results are not a valid fallback for the city-level chart.
        if (this.employeeLabels.length === 0 && result.overview?.topEmployees?.length) {
          this.buildEmployeeChart(this.seriesFromPoints('Employee Analytics', 'Employee', 'Employee', result.overview.topEmployees));
        }

        if (failedSections.length > 0) {
          this.errorMessage = `Some sections could not be loaded (${failedSections.join(', ')}). Check the SSAS connection and backend logs.`;
        }

        this.loading = false;
      },
      error: () => {
        this.clearDashboard();
        this.errorMessage = 'Unable to load dashboard. Verify backend http://localhost:5084 and SSAS connection.';
        this.loading = false;
      }
    });
  }

  private buildTrendChart(series: AnalyticsSeriesResponseDto): void {
    const points = this.safePoints(series);
    this.trendLabels = points.map(point => this.safeLabel(point.label));
    this.trendDatasets = [
      { label: 'Total Amount', data: points.map(point => this.metric(point, 'totalAmount')), colorKey: 'blue', yAxisID: 'y' },
      { label: 'Order Quantity', data: points.map(point => this.metric(point, 'quantity')), colorKey: 'yellow', yAxisID: 'y1' }
    ];
  }

  private buildProductChart(series: AnalyticsSeriesResponseDto): void {
    const points = this.safePoints(series);
    this.productLabels = points.map(point => this.safeLabel(point.label));
    this.productDatasets = [
      { label: 'Revenue', data: points.map(point => this.metric(point, 'totalAmount')), colorKey: 'blue', yAxisID: 'x' },
      { label: 'Order Quantity', data: points.map(point => this.metric(point, 'quantity')), colorKey: 'yellow', yAxisID: 'x1' }
    ];
  }

  private buildCityChart(series: AnalyticsSeriesResponseDto): void {
    const points = this.safePoints(series);
    this.cityLabels = points.map(point => this.safeLabel(point.label));
    this.cityDatasets = [
      { label: 'Revenue by city', data: points.map(point => this.metric(point, 'totalAmount')), colorKey: 'multi' }
    ];
  }

  private buildEmployeeChart(series: AnalyticsSeriesResponseDto): void {
    const points = this.safePoints(series);
    this.employeeLabels = points.map(point => this.safeLabel(point.label));
    this.employeeDatasets = [
      { label: 'Revenue', data: points.map(point => this.metric(point, 'totalAmount')), colorKey: 'green', yAxisID: 'x' }
    ];
  }

  private toKpiCards(overview: DashboardOverviewDto): KpiViewModel[] {
    const kpis = overview.kpis ?? {};

    const cards: KpiViewModel[] = [
      { title: 'Montant total en K', value: this.format.money(kpis.amountInK, ' K'), subtitle: 'SSAS measure: Montant total en K', icon: '↗', tone: 'blue', rawValue: kpis.amountInK },
      { title: 'Total Amount', value: this.format.money(kpis.totalAmount), subtitle: 'SSAS measure: Total Amount', icon: '💰', tone: 'blue', rawValue: kpis.totalAmount },
      { title: 'Orders', value: this.format.number(kpis.orders), subtitle: 'SSAS measure: Nombre totale de commandes', icon: '🛒', tone: 'yellow', rawValue: kpis.orders },
      { title: 'Order Quantity', value: this.format.number(kpis.quantity), subtitle: 'SSAS measure: Order Quantity', icon: '▣', tone: 'green', rawValue: kpis.quantity },
      { title: 'Panier moyen', value: this.format.money(kpis.averageBasket), subtitle: 'SSAS measure: Panier moyen', icon: '◈', tone: 'gray', rawValue: kpis.averageBasket },
      { title: 'Evolution CA %', value: this.format.percent(kpis.revenueGrowthPercent), subtitle: 'SSAS measure: Evolution CA %', icon: '⇅', tone: kpis.revenueGrowthPercent !== null && kpis.revenueGrowthPercent !== undefined && kpis.revenueGrowthPercent < 0 ? 'red' : 'green', rawValue: kpis.revenueGrowthPercent }
    ];

    return cards.filter(card => this.hasValue(card.rawValue));
  }

  private hasValue(value: number | null | undefined): boolean {
    return value !== null && value !== undefined && Number.isFinite(Number(value));
  }

  private toCustomerRows(points: AnalyticsPointDto[]): Array<Record<string, unknown>> {
    return this.safePointArray(points).map((point, index) => ({
      rank: index + 1,
      label: this.safeLabel(point.label),
      totalAmount: this.metric(point, 'totalAmount'),
      orders: this.metric(point, 'orders'),
      averageBasket: this.metric(point, 'averageBasket')
    }));
  }

  private safePoints(series: AnalyticsSeriesResponseDto | null | undefined): AnalyticsPointDto[] {
    if (!series || !Array.isArray(series.points)) {
      return [];
    }
    return series.points.filter(point => !!point && !!point.metrics);
  }

  private safePointArray(points: AnalyticsPointDto[] | null | undefined): AnalyticsPointDto[] {
    if (!Array.isArray(points)) {
      return [];
    }
    return points.filter(point => !!point && !!point.metrics);
  }

  private metric(point: AnalyticsPointDto, key: keyof AnalyticsPointDto['metrics']): number {
    const value = point.metrics?.[key];
    if (value === null || value === undefined || Number.isNaN(Number(value))) {
      return 0;
    }
    return Number(value);
  }

  private safeLabel(value: string | null | undefined): string {
    return value && value.trim() ? value.trim() : 'Unknown';
  }

  private isEmptySeries(series: AnalyticsSeriesResponseDto | null | undefined): boolean {
    return !series || !Array.isArray(series.points) || series.points.length === 0;
  }

  private seriesFromPoints(analyticalArea: string, axis: string, level: string, points: AnalyticsPointDto[]): AnalyticsSeriesResponseDto {
    return {
      cubeName: 'CUBE_EDB',
      analyticalArea,
      axis,
      level,
      mdx: 'Fallback from /api/dashboard/overview',
      generatedAtUtc: new Date().toISOString(),
      points
    };
  }

  private emptySeries(analyticalArea = '', axis = '', level = ''): AnalyticsSeriesResponseDto {
    return {
      cubeName: 'CUBE_EDB',
      analyticalArea,
      axis,
      level,
      mdx: '',
      generatedAtUtc: new Date().toISOString(),
      points: []
    };
  }

  private clearDashboard(): void {
    this.overview = undefined;
    this.kpiCards = [];
    this.trendLabels = [];
    this.trendDatasets = [];
    this.productLabels = [];
    this.productDatasets = [];
    this.cityLabels = [];
    this.cityDatasets = [];
    this.employeeLabels = [];
    this.employeeDatasets = [];
    this.customerRows = [];
  }
}
