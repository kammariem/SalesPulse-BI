import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AnalyticsFilterDto,
  AnalyticsMetricDto,
  AnalyticsPointDto,
  AnalyticsSeriesResponseDto,
  CustomerLevel,
  DashboardOverviewDto,
  DimensionMemberDto,
  DimensionMembersResponseDto,
  EmployeeLevel,
  GeographyLevel,
  MeasureKey,
  MetricValuesDto,
  ProductLevel,
  SalesDrilldownAxis,
  SsasConnectionStatusDto,
  TimeGrain
} from '../models/analytics.models';

@Injectable({ providedIn: 'root' })
export class AnalyticsApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getHealth(): Observable<unknown> {
    return this.http.get(`${this.baseUrl}/health`).pipe(catchError(this.handleError));
  }

  testSsas(): Observable<SsasConnectionStatusDto> {
    return this.http.get<SsasConnectionStatusDto>(`${this.baseUrl}/ssas/test`).pipe(catchError(this.handleError));
  }

  getDashboardOverview(filters: AnalyticsFilterDto = {}): Observable<DashboardOverviewDto> {
    return this.http.get<unknown>(`${this.baseUrl}/dashboard/overview`, {
      params: this.toParams(filters)
    }).pipe(
      map((response) => this.normalizeOverview(response)),
      catchError(this.handleError)
    );
  }

  getKpis(filters: AnalyticsFilterDto = {}): Observable<AnalyticsMetricDto[]> {
    return this.http.get<AnalyticsMetricDto[]>(`${this.baseUrl}/analytics/kpis`, {
      params: this.toParams(filters)
    }).pipe(catchError(this.handleError));
  }

  getSalesTrend(grain: TimeGrain | number = 'Month', filters: AnalyticsFilterDto = {}): Observable<AnalyticsSeriesResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/sales/trend`, {
      // Use numeric enum values because Swagger confirmed numeric values are accepted by the .NET backend.
      params: this.toParams({ ...filters, grain: this.timeGrainCode(grain) })
    }).pipe(
      map((response) => this.normalizeSeries(response, 'Sales Trend', 'Date', String(grain))),
      catchError(this.handleError)
    );
  }

  getSalesDrilldown(
    axis: SalesDrilldownAxis | number,
    level: string | number,
    measure: MeasureKey | number = 'TotalAmount',
    top = 20,
    filters: AnalyticsFilterDto = {}
  ): Observable<AnalyticsSeriesResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/sales/drilldown`, {
      params: this.toParams({ ...filters, axis: this.axisCode(axis), level, measure: this.measureCode(measure), top })
    }).pipe(
      map((response) => this.normalizeSeries(response, 'Sales Drilldown', String(axis), String(level))),
      catchError(this.handleError)
    );
  }

  getProductPerformance(
    level: ProductLevel | number = 'Category',
    measure: MeasureKey | number = 'TotalAmount',
    top = 10,
    filters: AnalyticsFilterDto = {}
  ): Observable<AnalyticsSeriesResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/products/performance`, {
      params: this.toParams({ ...filters, level: this.productLevelCode(level), measure: this.measureCode(measure), top })
    }).pipe(
      map((response) => this.normalizeSeries(response, 'Product Analytics', 'Product', String(level))),
      catchError(this.handleError)
    );
  }

  getCustomerPerformance(
    level: CustomerLevel | number = 'Category',
    measure: MeasureKey | number = 'TotalAmount',
    top = 10,
    filters: AnalyticsFilterDto = {}
  ): Observable<AnalyticsSeriesResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/customers/performance`, {
      params: this.toParams({ ...filters, level: this.customerLevelCode(level), measure: this.measureCode(measure), top })
    }).pipe(
      map((response) => this.normalizeSeries(response, 'Customer Analytics', 'Customer', String(level))),
      catchError(this.handleError)
    );
  }

  getEmployeePerformance(
    level: EmployeeLevel | number = 'Department',
    measure: MeasureKey | number = 'TotalAmount',
    top = 10,
    filters: AnalyticsFilterDto = {}
  ): Observable<AnalyticsSeriesResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/employees/performance`, {
      params: this.toParams({ ...filters, level: this.employeeLevelCode(level), measure: this.measureCode(measure), top })
    }).pipe(
      map((response) => this.normalizeSeries(response, 'Employee Analytics', 'Employee', String(level))),
      catchError(this.handleError)
    );
  }

  getGeographyPerformance(
    level: GeographyLevel | number = 'Country',
    measure: MeasureKey | number = 'TotalAmount',
    top = 25,
    filters: AnalyticsFilterDto = {}
  ): Observable<AnalyticsSeriesResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/geography/performance`, {
      params: this.toParams({ ...filters, level: this.geographyLevelCode(level), measure: this.measureCode(measure), top })
    }).pipe(
      map((response) => this.normalizeSeries(response, 'Geography Analytics', 'Geography', String(level))),
      catchError(this.handleError)
    );
  }

  getDimensionMembers(
    dimension: SalesDrilldownAxis,
    level: string,
    top = 200,
    filters: AnalyticsFilterDto = {}
  ): Observable<DimensionMembersResponseDto> {
    return this.http.get<unknown>(`${this.baseUrl}/dimensions/members`, {
      params: this.toParams({ ...filters, dimension: this.axisCode(dimension), level, top })
    }).pipe(
      map((response) => this.normalizeDimensionMembers(response, dimension, level)),
      catchError(this.handleError)
    );
  }

  private toParams(source: object | null | undefined): HttpParams {
    let params = new HttpParams();

    if (!source) {
      return params;
    }

    Object.entries(source).forEach(([key, rawValue]) => {
      if (rawValue === undefined || rawValue === null || rawValue === '') {
        return;
      }

      const value = String(rawValue).trim();
      if (!value || value.includes('&[]') || value.endsWith('.&[]')) {
        return;
      }

      if (key.toLowerCase().includes('uniquename') && !this.looksLikeSafeMemberUniqueName(value)) {
        return;
      }

      params = params.set(key, value);
    });

    return params;
  }

  private looksLikeSafeMemberUniqueName(value: string): boolean {
    return value.startsWith('[Dim ') && value.includes('].[') && !value.includes('[All]');
  }

  private normalizeOverview(raw: unknown): DashboardOverviewDto {
    const obj = this.asRecord(raw);
    return {
      cubeName: this.getString(obj, 'cubeName', 'CubeName') || 'CUBE_EDB',
      generatedAtUtc: this.getString(obj, 'generatedAtUtc', 'GeneratedAtUtc') || new Date().toISOString(),
      kpis: this.normalizeMetrics(this.get(obj, 'kpis', 'Kpis')),
      topProducts: this.normalizePoints(this.get(obj, 'topProducts', 'TopProducts')),
      topCustomers: this.normalizePoints(this.get(obj, 'topCustomers', 'TopCustomers')),
      topCountries: this.normalizePoints(this.get(obj, 'topCountries', 'TopCountries')),
      topEmployees: this.normalizePoints(this.get(obj, 'topEmployees', 'TopEmployees'))
    };
  }

  private normalizeSeries(raw: unknown, area: string, axis: string, level: string): AnalyticsSeriesResponseDto {
    const obj = this.asRecord(raw);
    const points = this.normalizePoints(this.get(obj, 'points', 'Points'));

    return {
      cubeName: this.getString(obj, 'cubeName', 'CubeName') || 'CUBE_EDB',
      analyticalArea: this.getString(obj, 'analyticalArea', 'AnalyticalArea') || area,
      axis: this.getString(obj, 'axis', 'Axis') || axis,
      level: this.getString(obj, 'level', 'Level') || level,
      mdx: this.getString(obj, 'mdx', 'Mdx') || '',
      generatedAtUtc: this.getString(obj, 'generatedAtUtc', 'GeneratedAtUtc') || new Date().toISOString(),
      points
    };
  }

  private normalizePoints(raw: unknown): AnalyticsPointDto[] {
    if (!Array.isArray(raw)) {
      return [];
    }

    return raw
      .map((item) => {
        const obj = this.asRecord(item);
        return {
          label: this.getString(obj, 'label', 'Label') || 'Unknown',
          uniqueName: this.getString(obj, 'uniqueName', 'UniqueName'),
          metrics: this.normalizeMetrics(this.get(obj, 'metrics', 'Metrics'))
        } satisfies AnalyticsPointDto;
      })
      .filter((point) => point.label !== 'Unknown' || this.hasAnyMetric(point.metrics));
  }

  private normalizeMetrics(raw: unknown): MetricValuesDto {
    const obj = this.asRecord(raw);
    return {
      totalAmount: this.getNumber(obj, 'totalAmount', 'TotalAmount'),
      quantity: this.getNumber(obj, 'quantity', 'Quantity'),
      orders: this.getNumber(obj, 'orders', 'Orders'),
      salesLines: this.getNumber(obj, 'salesLines', 'SalesLines'),
      averageBasket: this.getNumber(obj, 'averageBasket', 'AverageBasket'),
      averageQuantityPerOrder: this.getNumber(obj, 'averageQuantityPerOrder', 'AverageQuantityPerOrder'),
      averageUnitPrice: this.getNumber(obj, 'averageUnitPrice', 'AverageUnitPrice'),
      revenuePerActiveCustomer: this.getNumber(obj, 'revenuePerActiveCustomer', 'RevenuePerActiveCustomer'),
      amountInK: this.getNumber(obj, 'amountInK', 'AmountInK'),
      previousYearRevenue: this.getNumber(obj, 'previousYearRevenue', 'PreviousYearRevenue'),
      revenueGrowthPercent: this.getNumber(obj, 'revenueGrowthPercent', 'RevenueGrowthPercent'),
      yearToDateRevenue: this.getNumber(obj, 'yearToDateRevenue', 'YearToDateRevenue')
    };
  }

  private normalizeDimensionMembers(raw: unknown, dimension: string, level: string): DimensionMembersResponseDto {
    const obj = this.asRecord(raw);
    const membersRaw = this.get(obj, 'members', 'Members');
    const members: DimensionMemberDto[] = Array.isArray(membersRaw)
      ? membersRaw.map((item) => {
          const member = this.asRecord(item);
          return {
            label: this.getString(member, 'label', 'Label') || 'Unknown',
            uniqueName: this.getString(member, 'uniqueName', 'UniqueName'),
            totalAmount: this.getNumber(member, 'totalAmount', 'TotalAmount')
          };
        })
      : [];

    return {
      cubeName: this.getString(obj, 'cubeName', 'CubeName') || 'CUBE_EDB',
      dimension,
      level,
      generatedAtUtc: this.getString(obj, 'generatedAtUtc', 'GeneratedAtUtc') || new Date().toISOString(),
      members
    };
  }

  private hasAnyMetric(metrics: MetricValuesDto): boolean {
    return Object.values(metrics).some((value) => typeof value === 'number' && Number.isFinite(value));
  }

  private asRecord(value: unknown): Record<string, unknown> {
    return value && typeof value === 'object' ? (value as Record<string, unknown>) : {};
  }

  private get(obj: Record<string, unknown>, ...keys: string[]): unknown {
    for (const key of keys) {
      if (Object.prototype.hasOwnProperty.call(obj, key)) {
        return obj[key];
      }
    }
    return undefined;
  }

  private getString(obj: Record<string, unknown>, ...keys: string[]): string | null {
    const value = this.get(obj, ...keys);
    return value === undefined || value === null ? null : String(value);
  }

  private getNumber(obj: Record<string, unknown>, ...keys: string[]): number | null {
    const value = this.get(obj, ...keys);
    if (value === undefined || value === null || value === '') {
      return null;
    }
    const number = Number(value);
    return Number.isFinite(number) ? number : null;
  }

  private timeGrainCode(value: TimeGrain | number): number {
    if (typeof value === 'number') { return value; }
    return ({ Year: 0, Semester: 1, Quarter: 2, Month: 3, Date: 4 } as const)[value] ?? 3;
  }

  private productLevelCode(value: ProductLevel | number): number {
    if (typeof value === 'number') { return value; }
    return ({ Category: 0, Brand: 1, Product: 2 } as const)[value] ?? 0;
  }

  private customerLevelCode(value: CustomerLevel | number): number {
    if (typeof value === 'number') { return value; }
    return ({ Category: 0, CustomerType: 1, Company: 2, Customer: 3, DiscountPercent: 4 } as const)[value] ?? 0;
  }

  private employeeLevelCode(value: EmployeeLevel | number): number {
    if (typeof value === 'number') { return value; }
    return ({ Department: 0, JobTitle: 1, Employee: 2 } as const)[value] ?? 0;
  }

  private geographyLevelCode(value: GeographyLevel | number): number {
    if (typeof value === 'number') { return value; }
    return ({ Country: 0, City: 1 } as const)[value] ?? 0;
  }

  private axisCode(value: SalesDrilldownAxis | number): number {
    if (typeof value === 'number') { return value; }
    return ({ Date: 0, Product: 1, Customer: 2, Employee: 3, Geography: 4 } as const)[value] ?? 1;
  }

  private measureCode(value: MeasureKey | number): number {
    if (typeof value === 'number') { return value; }
    return ({
      TotalAmount: 0,
      Quantity: 1,
      Orders: 2,
      SalesLines: 3,
      AverageBasket: 4,
      AverageQuantityPerOrder: 5,
      AverageUnitPrice: 6,
      RevenuePerActiveCustomer: 7,
      AmountInK: 8,
      PreviousYearRevenue: 9,
      RevenueGrowthPercent: 10,
      YearToDateRevenue: 11
    } as const)[value] ?? 0;
  }

  private handleError(error: unknown): Observable<never> {
    return throwError(() => error);
  }
}
