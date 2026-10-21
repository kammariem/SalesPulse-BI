import { Component, Input } from '@angular/core';
import { TableColumn } from '../../../core/models/analytics.models';
import { FormatService } from '../../../core/services/format.service';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.css']
})
export class DataTableComponent {
  @Input() title = '';
  @Input() subtitle = '';
  @Input() columns: TableColumn[] = [];
  @Input() rows: Array<Record<string, unknown>> = [];
  @Input() loading = false;

  constructor(private readonly format: FormatService) {}

  display(value: unknown, type: TableColumn['type']): string {
    if (value === null || value === undefined || value === '') return '—';
    if (type === 'money') return this.format.money(Number(value));
    if (type === 'number') return this.format.number(Number(value));
    if (type === 'percent') return this.format.percent(Number(value));
    return value === null || value === undefined || value === '' ? '—' : String(value);
  }
}
