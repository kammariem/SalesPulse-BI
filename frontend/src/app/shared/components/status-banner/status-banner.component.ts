import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-status-banner',
  templateUrl: './status-banner.component.html',
  styleUrls: ['./status-banner.component.css']
})
export class StatusBannerComponent {
  @Input() type: 'info' | 'error' | 'success' = 'info';
  @Input() title = '';
  @Input() message = '';
}
