import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { AnalyticsApiService } from '../../../core/services/analytics-api.service';

interface SearchShortcut {
  label: string;
  description: string;
  route: string;
  keywords: string[];
}

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  @Output() toggleSidebar = new EventEmitter<void>();

  backendOnline: boolean | null = null;
  ssasOnline: boolean | null = null;
  search = '';
  showSuggestions = false;

  readonly shortcuts: SearchShortcut[] = [
    { label: 'Dashboard', description: 'KPI overview, top products, countries and employees', route: '/dashboard', keywords: ['dashboard', 'overview', 'kpi', 'home', 'resume', 'summary'] },
    { label: 'Sales trend', description: 'Time-series analysis by year, quarter, month or date', route: '/sales', keywords: ['sales', 'trend', 'time', 'date', 'month', 'quarter', 'ca', 'revenue'] },
    { label: 'Products', description: 'Product, category and brand performance', route: '/products', keywords: ['product', 'products', 'category', 'brand', 'produit', 'article'] },
    { label: 'Customers', description: 'Customer ranking by category, company or customer type', route: '/customers', keywords: ['customer', 'customers', 'client', 'company', 'type'] },
    { label: 'Geography', description: 'Country and city segmentation', route: '/geography', keywords: ['country', 'city', 'geography', 'region', 'pays', 'ville'] },
    { label: 'Employees', description: 'Department, job title and employee analysis', route: '/employees', keywords: ['employee', 'employees', 'staff', 'department', 'job', 'team'] },
    { label: 'Dimensions', description: 'SSAS members and MEMBER_UNIQUE_NAME dictionary', route: '/dimensions', keywords: ['dimension', 'member', 'unique', 'filter', 'hierarchy', 'ssas'] }
  ];

  constructor(private readonly api: AnalyticsApiService, private readonly router: Router) {}

  ngOnInit(): void {
    this.refreshStatus();
  }

  refreshStatus(): void {
    this.api.getHealth().subscribe({
      next: () => this.backendOnline = true,
      error: () => this.backendOnline = false
    });

    this.api.testSsas().subscribe({
      next: response => this.ssasOnline = response.isConnected,
      error: () => this.ssasOnline = false
    });
  }

  get filteredShortcuts(): SearchShortcut[] {
    const query = this.search.trim().toLowerCase();
    if (!query) {
      return this.shortcuts.slice(0, 5);
    }

    return this.shortcuts.filter(item =>
      item.label.toLowerCase().includes(query)
      || item.description.toLowerCase().includes(query)
      || item.keywords.some(keyword => keyword.includes(query))
    ).slice(0, 6);
  }

  runSearch(): void {
    const first = this.filteredShortcuts[0];
    if (first) {
      this.goTo(first);
    }
  }

  goTo(item: SearchShortcut): void {
    this.search = item.label;
    this.showSuggestions = false;
    this.router.navigateByUrl(item.route);
  }
}
