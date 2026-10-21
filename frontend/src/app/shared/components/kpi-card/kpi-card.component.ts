import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-kpi-card',
  templateUrl: './kpi-card.component.html',
  styleUrls: ['./kpi-card.component.css']
})
export class KpiCardComponent {
  @Input() title = '';
  @Input() value = '—';
  @Input() subtitle = '';
  @Input() icon = '▣';
  @Input() tone: 'blue' | 'yellow' | 'green' | 'red' | 'gray' = 'blue';
  @Input() loading = false;
}
