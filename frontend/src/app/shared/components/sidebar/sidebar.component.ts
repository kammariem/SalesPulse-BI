import { Component, Input } from '@angular/core';

interface NavItem {
  label: string;
  route: string;
  icon: string;
  badge?: string;
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  @Input() collapsed = false;

  readonly mainItems: NavItem[] = [
    { label: 'Dashboard', route: '/dashboard', icon: '▦' },
    { label: 'Sales', route: '/sales', icon: '↗' },
    { label: 'Products', route: '/products', icon: '◧' },
    { label: 'Customers', route: '/customers', icon: '◉' },
    { label: 'Geography', route: '/geography', icon: '⌖' },
    { label: 'Employees', route: '/employees', icon: '♙' },
    // { label: 'Dimensions', route: '/dimensions', icon: '▤', badge: 'filters' }
  ];
}
