import { Component, OnInit } from '@angular/core';
import { DimensionMemberDto, SalesDrilldownAxis, TableColumn } from '../../core/models/analytics.models';
import { AnalyticsApiService } from '../../core/services/analytics-api.service';

@Component({ selector: 'app-dimensions', templateUrl: './dimensions.component.html', styleUrls: ['./dimensions.component.css'] })
export class DimensionsComponent implements OnInit {
  loading = false;
  errorMessage = '';
  dimension: SalesDrilldownAxis = 'Product';
  level = 'Category';
  top = 100;
  members: DimensionMemberDto[] = [];
  rows: Array<Record<string, unknown>> = [];
  dimensions: SalesDrilldownAxis[] = ['Date', 'Product', 'Customer', 'Employee', 'Geography'];
  readonly defaultLevels: Record<SalesDrilldownAxis, string[]> = {
    Date: ['Year', 'Semester', 'Quarter', 'Month', 'Date'],
    Product: ['Category', 'Brand', 'Product'],
    Customer: ['Category', 'CustomerType', 'Company', 'Customer', 'DiscountPercent'],
    Employee: ['Department', 'JobTitle', 'Employee'],
    Geography: ['Country', 'City']
  };
  columns: TableColumn[] = [
    { key: 'rank', label: '#', type: 'text' },
    { key: 'label', label: 'Caption', type: 'text' },
    { key: 'uniqueName', label: 'MEMBER_UNIQUE_NAME', type: 'text' },
    { key: 'totalAmount', label: 'Total Amount', type: 'money' }
  ];

  constructor(private readonly api: AnalyticsApiService) {}
  ngOnInit(): void { this.load(); }
  onDimensionChange(): void { this.level = this.defaultLevels[this.dimension][0]; this.load(); }
  load(): void {
    this.loading = true; this.errorMessage = '';
    this.api.getDimensionMembers(this.dimension, this.level, this.top).subscribe({
      next: res => {
        this.members = res.members;
        this.rows = res.members.map(m => ({ label: m.label, uniqueName: m.uniqueName ?? '', totalAmount: m.totalAmount }));
        this.loading = false;
      },
      error: () => {
        this.members = [];
        this.rows = [];
        this.errorMessage = 'Unable to load dimension members. Check the selected level and SSAS connection.';
        this.loading = false;
      }
    });
  }
}
