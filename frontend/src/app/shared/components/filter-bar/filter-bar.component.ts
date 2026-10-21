import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { catchError, forkJoin, of } from 'rxjs';
import { AnalyticsFilterDto, DimensionMemberDto } from '../../../core/models/analytics.models';
import { AnalyticsApiService } from '../../../core/services/analytics-api.service';

@Component({
  selector: 'app-filter-bar',
  templateUrl: './filter-bar.component.html',
  styleUrls: ['./filter-bar.component.css']
})
export class FilterBarComponent implements OnInit, OnChanges {
  @Input() filters: AnalyticsFilterDto = {};
  @Output() filtersChange = new EventEmitter<AnalyticsFilterDto>();
  @Output() refresh = new EventEmitter<void>();

  years: DimensionMemberDto[] = [];
  countries: DimensionMemberDto[] = [];
  categories: DimensionMemberDto[] = [];
  loading = false;

  selectedYear = '';
  selectedCountry = '';
  selectedCategory = '';

  constructor(private readonly api: AnalyticsApiService) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['filters']) {
      this.selectedYear = this.filters.yearUniqueName ?? '';
      this.selectedCountry = this.filters.countryUniqueName ?? '';
      this.selectedCategory = this.filters.productCategoryUniqueName ?? '';
    }
  }

  loadMembers(): void {
    this.loading = true;
    forkJoin({
      years: this.api.getDimensionMembers('Date', 'Year', 20).pipe(catchError(() => of({ members: [] } as any))),
      countries: this.api.getDimensionMembers('Geography', 'Country', 50).pipe(catchError(() => of({ members: [] } as any))),
      categories: this.api.getDimensionMembers('Product', 'Category', 50).pipe(catchError(() => of({ members: [] } as any)))
    }).subscribe({
      next: result => {
        this.years = result.years.members;
        this.countries = result.countries.members;
        this.categories = result.categories.members;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  apply(): void {
    this.filtersChange.emit({
      ...this.filters,
      yearUniqueName: this.cleanUniqueName(this.selectedYear),
      countryUniqueName: this.cleanUniqueName(this.selectedCountry),
      productCategoryUniqueName: this.cleanUniqueName(this.selectedCategory)
    });
    this.refresh.emit();
  }

  private cleanUniqueName(value: string): string | null {
    const cleaned = (value || '').trim();
    if (!cleaned || cleaned.includes('&[]') || cleaned.endsWith('.&[]')) {
      return null;
    }
    return cleaned;
  }

  clear(): void {
    this.selectedYear = '';
    this.selectedCountry = '';
    this.selectedCategory = '';
    this.filtersChange.emit({});
    this.refresh.emit();
  }
}
