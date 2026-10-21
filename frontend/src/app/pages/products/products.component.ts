import { Component, OnInit } from '@angular/core';
import { AnalyticsFilterDto, AnalyticsPointDto, MeasureKey, ProductLevel, TableColumn } from '../../core/models/analytics.models';
import { AnalyticsApiService } from '../../core/services/analytics-api.service';
import { FormatService } from '../../core/services/format.service';
import { ChartDatasetInput } from '../../shared/components/chart-panel/chart-panel.component';

@Component({ selector: 'app-products', templateUrl: './products.component.html', styleUrls: ['./products.component.css'] })
export class ProductsComponent implements OnInit {
  filters: AnalyticsFilterDto = {};
  loading = false;
  errorMessage = '';
  level: ProductLevel = 'Category';
  measure: MeasureKey = 'TotalAmount';
  top = 15;
  labels: string[] = [];
  datasets: ChartDatasetInput[] = [];
  shareDatasets: ChartDatasetInput[] = [];
  rows: Array<Record<string, unknown>> = [];
  levels: ProductLevel[] = ['Category', 'Brand', 'Product'];
  measures: MeasureKey[] = ['TotalAmount', 'Quantity'];
  columns: TableColumn[] = [
    { key: 'rank', label: '#', type: 'text' },
    { key: 'label', label: 'Product member', type: 'text' },
    { key: 'totalAmount', label: 'Revenue', type: 'money' },
    { key: 'quantity', label: 'Quantity', type: 'number' },
    // { key: 'orders', label: 'Orders', type: 'number' },
    // { key: 'avgUnit', label: 'Avg unit price', type: 'money' }
  ];

  constructor(private readonly api: AnalyticsApiService, private readonly format: FormatService) {}
  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.api.getProductPerformance(this.level, this.measure, this.top, this.filters).subscribe({
      next: res => {
        this.labels = res.points.map(p => p.label);
        this.datasets = [
          { label: 'Revenue', colorKey: 'blue', data: res.points.map(p => this.format.pointValue(p, 'totalAmount')) },
          { label: 'Quantity', colorKey: 'yellow', data: res.points.map(p => this.format.pointValue(p, 'quantity')) }
        ]; this.shareDatasets = this.datasets.length ? [this.datasets[0]] : [];
        this.rows = this.toRows(res.points);
        this.loading = false;
      },
      error: () => { this.errorMessage = 'Product performance endpoint failed.'; this.loading = false; }
    });
  }

  private toRows(points: AnalyticsPointDto[]): Array<Record<string, unknown>> {
    return points.map(p => ({ label: p.label, totalAmount: p.metrics.totalAmount ?? 0, quantity: p.metrics.quantity ?? 0, orders: p.metrics.orders ?? 0, avgUnit: p.metrics.averageUnitPrice ?? 0 }));
  }
}
