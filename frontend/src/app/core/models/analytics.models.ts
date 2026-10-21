export type MeasureKey =
  | 'TotalAmount'
  | 'Quantity'
  | 'Orders'
  | 'SalesLines'
  | 'AverageBasket'
  | 'AverageQuantityPerOrder'
  | 'AverageUnitPrice'
  | 'RevenuePerActiveCustomer'
  | 'AmountInK'
  | 'PreviousYearRevenue'
  | 'RevenueGrowthPercent'
  | 'YearToDateRevenue';

export type TimeGrain = 'Year' | 'Semester' | 'Quarter' | 'Month' | 'Date';
export type ProductLevel = 'Category' | 'Brand' | 'Product';
export type CustomerLevel = 'Category' | 'CustomerType' | 'Company' | 'Customer' | 'DiscountPercent';
export type EmployeeLevel = 'Department' | 'JobTitle' | 'Employee';
export type GeographyLevel = 'Country' | 'City';
export type SalesDrilldownAxis = 'Date' | 'Product' | 'Customer' | 'Employee' | 'Geography';

export interface MetricValuesDto {
  totalAmount?: number | null;
  quantity?: number | null;
  orders?: number | null;
  salesLines?: number | null;
  averageBasket?: number | null;
  averageQuantityPerOrder?: number | null;
  averageUnitPrice?: number | null;
  revenuePerActiveCustomer?: number | null;
  amountInK?: number | null;
  previousYearRevenue?: number | null;
  revenueGrowthPercent?: number | null;
  yearToDateRevenue?: number | null;
}

export interface AnalyticsPointDto {
  label: string;
  uniqueName?: string | null;
  metrics: MetricValuesDto;
}

export interface AnalyticsMetricDto {
  key: string;
  caption: string;
  uniqueName: string;
  value?: number | null;
  format?: string | null;
}

export interface DashboardOverviewDto {
  cubeName: string;
  generatedAtUtc: string;
  kpis: MetricValuesDto;
  topProducts: AnalyticsPointDto[];
  topCustomers: AnalyticsPointDto[];
  topCountries: AnalyticsPointDto[];
  topEmployees: AnalyticsPointDto[];
}

export interface AnalyticsSeriesResponseDto {
  cubeName: string;
  analyticalArea: string;
  axis: string;
  level: string;
  mdx: string;
  generatedAtUtc: string;
  points: AnalyticsPointDto[];
}

export interface DimensionMemberDto {
  label: string;
  uniqueName?: string | null;
  totalAmount?: number | null;
}

export interface DimensionMembersResponseDto {
  cubeName: string;
  dimension: string;
  level: string;
  generatedAtUtc: string;
  members: DimensionMemberDto[];
}

export interface AnalyticsFilterDto {
  yearUniqueName?: string | null;
  semesterUniqueName?: string | null;
  quarterUniqueName?: string | null;
  monthUniqueName?: string | null;
  dateUniqueName?: string | null;

  productCategoryUniqueName?: string | null;
  brandUniqueName?: string | null;
  productUniqueName?: string | null;

  customerCategoryUniqueName?: string | null;
  customerTypeUniqueName?: string | null;
  companyUniqueName?: string | null;
  customerUniqueName?: string | null;
  discountPercentUniqueName?: string | null;

  departmentUniqueName?: string | null;
  jobTitleUniqueName?: string | null;
  employeeUniqueName?: string | null;

  countryUniqueName?: string | null;
  cityUniqueName?: string | null;
}

export interface DashboardFilters extends AnalyticsFilterDto {}

export interface ApiHealthDto {
  status?: string;
  app?: string;
  timeUtc?: string;
}

export interface SsasConnectionStatusDto {
  isConnected: boolean;
  message: string;
}

export interface KpiViewModel {
  title: string;
  value: string;
  subtitle: string;
  icon: string;
  tone: 'blue' | 'yellow' | 'green' | 'red' | 'gray';
  rawValue?: number | null;
}

export interface TableColumn {
  key: string;
  label: string;
  type?: 'text' | 'number' | 'money' | 'percent';
}
