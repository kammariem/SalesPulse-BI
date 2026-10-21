import { Injectable } from '@angular/core';
import { AnalyticsPointDto, MetricValuesDto } from '../models/analytics.models';

@Injectable({ providedIn: 'root' })
export class FormatService {
  private readonly moneyFormatter = new Intl.NumberFormat('fr-FR', {
    maximumFractionDigits: 2,
    minimumFractionDigits: 0
  });

  private readonly numberFormatter = new Intl.NumberFormat('fr-FR', {
    maximumFractionDigits: 2,
    minimumFractionDigits: 0
  });

  money(value?: number | null, suffix = ''): string {
    if (value === null || value === undefined || Number.isNaN(Number(value))) {
      return '—';
    }
    return `${this.moneyFormatter.format(Number(value))}${suffix}`;
  }

  number(value?: number | null): string {
    if (value === null || value === undefined || Number.isNaN(Number(value))) {
      return '—';
    }
    return this.numberFormatter.format(Number(value));
  }

  percent(value?: number | null): string {
    if (value === null || value === undefined || Number.isNaN(Number(value))) {
      return '—';
    }
    const normalized = Math.abs(value) <= 1 ? value * 100 : value;
    return `${this.numberFormatter.format(normalized)}%`;
  }

  metricValue(metrics: MetricValuesDto | undefined, key: keyof MetricValuesDto): number {
    const value = metrics?.[key];
    return value === null || value === undefined ? 0 : Number(value);
  }

  pointValue(point: AnalyticsPointDto, key: keyof MetricValuesDto = 'totalAmount'): number {
    return this.metricValue(point.metrics, key);
  }

  label(value?: string | null): string {
    return value && value.trim().length > 0 ? value : 'Unknown';
  }
}
