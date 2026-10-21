import { Component, OnInit } from '@angular/core';
import { AnalyticsFilterDto, AnalyticsPointDto, MeasureKey, SalesDrilldownAxis, TableColumn, TimeGrain } from '../../core/models/analytics.models';
import { AnalyticsApiService } from '../../core/services/analytics-api.service';
import { FormatService } from '../../core/services/format.service';
import { ChartDatasetInput } from '../../shared/components/chart-panel/chart-panel.component';

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.css']
})
export class SalesComponent implements OnInit {
  loadingTrend = false;
  loadingDrill = false;
  errorMessage = '';
  filters: AnalyticsFilterDto = {};

  grain: TimeGrain = 'Month';
  axis: SalesDrilldownAxis = 'Product';
  level = 'Category';
  measure: MeasureKey = 'TotalAmount';
  top = 15;

  trendLabels: string[] = [];
  trendDatasets: ChartDatasetInput[] = [];
  drillLabels: string[] = [];
  drillDatasets: ChartDatasetInput[] = [];
  rows: Array<Record<string, unknown>> = [];

  readonly grains: TimeGrain[] = ['Year', 'Semester', 'Quarter', 'Month', 'Date'];
  readonly axes: SalesDrilldownAxis[] = ['Date', 'Product', 'Customer', 'Employee', 'Geography'];
  readonly measures: MeasureKey[] = ['TotalAmount', 'Quantity'];

  get levelOptions(): string[] {
    const options: Record<SalesDrilldownAxis, string[]> = {
      Date: ['Year', 'Semester', 'Quarter', 'Month', 'Date'],
      Product: ['Category', 'Brand', 'Product'],
      Customer: ['Category', 'CustomerType', 'Company', 'Customer', 'DiscountPercent'],
      Employee: ['Department', 'JobTitle', 'Employee'],
      Geography: ['Country', 'City']
    };
    return options[this.axis];
  }

  readonly columns: TableColumn[] = [
    { key: 'rank', label: '#', type: 'text' },
    { key: 'label', label: 'Member', type: 'text' },
    { key: 'totalAmount', label: 'Total Amount', type: 'money' },
    { key: 'quantity', label: 'Quantity', type: 'number' },
    // { key: 'orders', label: 'Orders', type: 'number' },
    // { key: 'growth', label: 'Evolution CA %', type: 'percent' }
  ];

  constructor(private readonly api: AnalyticsApiService, private readonly format: FormatService) {}

  ngOnInit(): void {
    this.loadAll();
  }

  onAxisChange(): void {
    this.level = this.levelOptions[0];
  }

  loadAll(): void {
    this.loadTrend();
    this.loadDrilldown();
  }

  loadTrend(): void {
    this.loadingTrend = true;
    this.errorMessage = '';
    this.api.getSalesTrend(this.grain, this.filters).subscribe({
      next: response => {
        this.trendLabels = response.points.map(p => p.label);
        this.trendDatasets = [
          { label: 'Total Amount', colorKey: 'blue', data: response.points.map(p => this.format.pointValue(p, 'totalAmount')) },
          { label: 'Quantity', colorKey: 'yellow', data: response.points.map(p => this.format.pointValue(p, 'quantity')) }
        ];
        this.loadingTrend = false;
      },
      error: () => {
        this.trendLabels = [];
        this.trendDatasets = [];
        this.errorMessage = 'Sales trend could not be loaded. Check the SSAS connection or selected period.';
        this.loadingTrend = false;
      }
    });
  }

  loadDrilldown(): void {
    this.loadingDrill = true;
    this.api.getSalesDrilldown(this.axis, this.level, this.measure, this.top, this.filters).subscribe({
      next: response => {
        this.drillLabels = response.points.map(p => p.label);
        this.drillDatasets = [{ label: this.measure, colorKey: 'blue', data: response.points.map(p => this.pickMeasure(p)) }];
        this.rows = this.toRows(response.points);
        this.loadingDrill = false;
      },
      error: () => {
        this.drillLabels = [];
        this.drillDatasets = [];
        this.rows = [];
        this.errorMessage = 'Sales drilldown could not be loaded. Check the selected dimension and SSAS connection.';
        this.loadingDrill = false;
      }
    });
  }

  private pickMeasure(point: AnalyticsPointDto): number {
    const map: Record<MeasureKey, keyof AnalyticsPointDto['metrics']> = {
      TotalAmount: 'totalAmount', Quantity: 'quantity', Orders: 'orders', SalesLines: 'salesLines',
      AverageBasket: 'averageBasket', AverageQuantityPerOrder: 'averageQuantityPerOrder', AverageUnitPrice: 'averageUnitPrice',
      RevenuePerActiveCustomer: 'revenuePerActiveCustomer', AmountInK: 'amountInK', PreviousYearRevenue: 'previousYearRevenue',
      RevenueGrowthPercent: 'revenueGrowthPercent', YearToDateRevenue: 'yearToDateRevenue'
    };
    return this.format.pointValue(point, map[this.measure]);
  }

  private toRows(points: AnalyticsPointDto[]): Array<Record<string, unknown>> {
    return points.map(p => ({
      label: p.label,
      totalAmount: p.metrics.totalAmount ?? 0,
      quantity: p.metrics.quantity ?? 0,
      orders: p.metrics.orders ?? 0,
      growth: p.metrics.revenueGrowthPercent ?? 0
    }));
  }
}
