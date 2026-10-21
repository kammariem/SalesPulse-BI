import {
  AfterViewInit,
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnDestroy,
  ViewChild
} from '@angular/core';

import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

export interface ChartDatasetInput {
  label: string;
  data: number[];
  colorKey?: 'blue' | 'yellow' | 'green' | 'orange' | 'purple' | 'multi';
  yAxisID?: 'y' | 'y1' | 'x' | 'x1';
}

@Component({
  selector: 'app-chart-panel',
  templateUrl: './chart-panel.component.html',
  styleUrls: ['./chart-panel.component.css']
})
export class ChartPanelComponent implements AfterViewInit, OnChanges, OnDestroy {
  @ViewChild('canvas') canvas?: ElementRef<HTMLCanvasElement>;

  @Input() title = '';
  @Input() subtitle = '';
  @Input() badgeLabel = 'Live cube';

  @Input() type: 'line' | 'bar' | 'doughnut' = 'line';
  @Input() chartType: 'line' | 'bar' | 'doughnut' | '' = '';
  @Input() horizontal = false;

  @Input() labels: string[] = [];
  @Input() datasets: ChartDatasetInput[] = [];

  @Input() height: number | string = 360;
  @Input() loading = false;

  @Input() reference = '';
  @Input() annotation = '';
  @Input() graphRef = '';
  @Input() note = '';
  @Input() emptyMessage = 'No cube data returned for this visual.';

  private chart?: Chart;

  ngAfterViewInit(): void {
    setTimeout(() => this.render(), 0);
  }

  ngOnChanges(): void {
    setTimeout(() => this.render(), 0);
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  get resolvedType(): 'line' | 'bar' | 'doughnut' {
    return this.chartType || this.type || 'bar';
  }

  get resolvedReference(): string {
    return this.graphRef || this.reference;
  }

  get resolvedAnnotation(): string {
    return this.note || this.annotation;
  }

  get chartHeightPx(): number {
    const value = Number(this.height);
    return Number.isFinite(value) && value > 0 ? value : 360;
  }

  hasRenderableData(): boolean {
    if (this.loading) {
      return false;
    }

    return Array.isArray(this.labels)
      && this.labels.length > 0
      && Array.isArray(this.datasets)
      && this.datasets.some(dataset =>
        Array.isArray(dataset.data)
        && dataset.data.some(value => this.resolvedType === 'doughnut'
          ? Number.isFinite(Number(value)) && Number(value) > 0
          : Number.isFinite(Number(value)))
      );
  }

  private render(): void {
    if (!this.canvas) {
      return;
    }

    this.chart?.destroy();

    if (!this.hasRenderableData()) {
      return;
    }

    const context = this.canvas.nativeElement.getContext('2d');
    if (!context) {
      return;
    }

    const mapped = (this.datasets || []).map((dataset, index) => this.mapDataset(context, dataset, index));
    const hasSecondaryAxis = mapped.some((dataset: any) => dataset.yAxisID === 'y1' || dataset.xAxisID === 'x1');

    const config: any = {
      type: this.resolvedType,
      data: {
        labels: this.labels,
        datasets: mapped
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: this.resolvedType === 'bar' && this.horizontal ? 'y' : 'x',
        cutout: this.resolvedType === 'doughnut' ? '64%' : undefined,
        layout: {
          padding: { top: 8, right: 14, bottom: 8, left: 10 }
        },
        interaction: {
          mode: 'index',
          intersect: false
        },
        animation: {
          duration: 700,
          easing: 'easeOutQuart'
        },
        plugins: {
          legend: {
            display: true,
            position: 'top',
            align: 'center',
            labels: {
              color: '#334155',
              boxWidth: 10,
              boxHeight: 10,
              usePointStyle: true,
              padding: 18,
              font: { size: 12 }
            }
          },
          tooltip: {
            backgroundColor: '#0b1f4d',
            titleColor: '#ffffff',
            bodyColor: '#ffffff',
            borderColor: 'rgba(255,255,255,.16)',
            borderWidth: 1,
            padding: 12,
            cornerRadius: 12,
            callbacks: {
              label: (context: any) => {
                const label = context.dataset?.label || '';
                const value = Number(context.raw ?? 0);
                return `${label}: ${this.formatNumber(value)}`;
              }
            }
          }
        }
      }
    };

    if (this.resolvedType !== 'doughnut') {
      config.options.scales = this.buildScales(hasSecondaryAxis);
    }

    const ChartAny: any = Chart;
    this.chart = new ChartAny(context, config);
  }

  private buildScales(hasSecondaryAxis: boolean): any {
    if (this.resolvedType === 'bar' && this.horizontal) {
      const scales: any = {
        y: {
          grid: { display: false },
          ticks: {
            color: '#475569',
            font: { size: 11, weight: 600 }
          }
        },
        x: {
          beginAtZero: true,
          position: 'bottom',
          ticks: {
            color: '#2563eb',
            callback: (value: any) => this.compactNumber(Number(value))
          },
          grid: { color: 'rgba(148, 163, 184, 0.16)' },
          title: { display: true, text: 'Revenue', color: '#2563eb' }
        }
      };

      if (hasSecondaryAxis) {
        scales.x1 = {
          beginAtZero: true,
          position: 'top',
          ticks: {
            color: '#eab308',
            callback: (value: any) => this.compactNumber(Number(value))
          },
          grid: { drawOnChartArea: false },
          title: { display: true, text: 'Quantity', color: '#eab308' }
        };
      }

      return scales;
    }

    const scales: any = {
      x: {
        grid: { display: false },
        ticks: {
          color: '#64748b',
          maxRotation: 0,
          minRotation: 0,
          autoSkip: true,
          maxTicksLimit: 8
        }
      },
      y: {
        type: 'linear',
        position: 'left',
        beginAtZero: true,
        grid: { color: 'rgba(148, 163, 184, 0.18)' },
        ticks: {
          color: '#2563eb',
          callback: (value: any) => this.compactNumber(Number(value))
        },
        title: { display: true, text: 'Revenue', color: '#2563eb' }
      }
    };

    if (hasSecondaryAxis) {
      scales.y1 = {
        type: 'linear',
        position: 'right',
        beginAtZero: true,
        ticks: {
          color: '#eab308',
          callback: (value: any) => this.compactNumber(Number(value))
        },
        grid: { drawOnChartArea: false },
        title: { display: true, text: 'Quantity', color: '#eab308' }
      };
    }

    return scales;
  }

  private mapDataset(context: CanvasRenderingContext2D, dataset: ChartDatasetInput, index: number): any {
    if (this.resolvedType === 'doughnut') {
      return {
        label: dataset.label || `Series ${index + 1}`,
        data: dataset.data || [],
        backgroundColor: this.getDoughnutColors(),
        borderColor: '#ffffff',
        borderWidth: 3,
        hoverOffset: 10
      };
    }

    const palette = this.getPalette(context, dataset.colorKey || this.defaultColor(index));
    const axisKey = this.resolvedType === 'bar' && this.horizontal ? 'xAxisID' : 'yAxisID';

    return {
      label: dataset.label || `Series ${index + 1}`,
      data: dataset.data || [],
      borderColor: palette.border,
      backgroundColor: palette.fill,
      tension: 0.38,
      fill: this.resolvedType === 'line',
      borderWidth: this.resolvedType === 'line' ? 3 : 1.5,
      pointRadius: this.resolvedType === 'line' ? 4 : 0,
      pointHoverRadius: 6,
      borderRadius: this.resolvedType === 'bar' ? 12 : 0,
      barThickness: this.resolvedType === 'bar' ? 'flex' : undefined,
      maxBarThickness: this.resolvedType === 'bar' ? 34 : undefined,
      [axisKey]: dataset.yAxisID || (index > 0 ? (this.horizontal ? 'x1' : 'y1') : (this.horizontal ? 'x' : 'y'))
    };
  }

  private getPalette(
    context: CanvasRenderingContext2D,
    colorKey: string
  ): { fill: CanvasGradient | string | string[]; border: string } {
    if (colorKey === 'multi') {
      return { fill: this.getDoughnutColors(), border: '#2563eb' };
    }

    const gradient = context.createLinearGradient(0, 0, this.horizontal ? this.canvas?.nativeElement.width || 540 : 0, this.horizontal ? 0 : this.chartHeightPx);

    if (colorKey === 'yellow') {
      gradient.addColorStop(0, 'rgba(250, 204, 21, 0.96)');
      gradient.addColorStop(1, 'rgba(253, 224, 71, 0.28)');
      return { fill: gradient, border: '#eab308' };
    }

    if (colorKey === 'green') {
      gradient.addColorStop(0, 'rgba(16, 185, 129, 0.96)');
      gradient.addColorStop(1, 'rgba(110, 231, 183, 0.28)');
      return { fill: gradient, border: '#10b981' };
    }

    if (colorKey === 'orange') {
      gradient.addColorStop(0, 'rgba(245, 158, 11, 0.96)');
      gradient.addColorStop(1, 'rgba(252, 211, 77, 0.28)');
      return { fill: gradient, border: '#f59e0b' };
    }

    if (colorKey === 'purple') {
      gradient.addColorStop(0, 'rgba(139, 92, 246, 0.96)');
      gradient.addColorStop(1, 'rgba(196, 181, 253, 0.28)');
      return { fill: gradient, border: '#8b5cf6' };
    }

    gradient.addColorStop(0, 'rgba(37, 99, 235, 0.96)');
    gradient.addColorStop(1, 'rgba(96, 165, 250, 0.24)');
    return { fill: gradient, border: '#2563eb' };
  }

  private defaultColor(index: number): 'blue' | 'yellow' | 'green' | 'orange' {
    const colors: Array<'blue' | 'yellow' | 'green' | 'orange'> = ['blue', 'yellow', 'green', 'orange'];
    return colors[index % colors.length];
  }

  private getDoughnutColors(): string[] {
    return [
      '#1d4ed8', '#2563eb', '#3b82f6', '#60a5fa',
      '#fbbf24', '#f59e0b', '#10b981', '#34d399',
      '#8b5cf6', '#c084fc', '#06b6d4', '#14b8a6'
    ];
  }

  private compactNumber(value: number): string {
    if (!Number.isFinite(value)) {
      return '0';
    }
    const absolute = Math.abs(value);
    if (absolute >= 1_000_000) {
      return `${new Intl.NumberFormat('fr-FR', { maximumFractionDigits: 1 }).format(value / 1_000_000)}M`;
    }
    if (absolute >= 1_000) {
      return `${new Intl.NumberFormat('fr-FR', { maximumFractionDigits: 0 }).format(value / 1_000)}K`;
    }
    return new Intl.NumberFormat('fr-FR', { maximumFractionDigits: 0 }).format(value);
  }

  private formatNumber(value: number): string {
    return new Intl.NumberFormat('fr-FR', { maximumFractionDigits: 2 }).format(value);
  }
}
