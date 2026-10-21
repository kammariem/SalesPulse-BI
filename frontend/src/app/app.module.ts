import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { KpiCardComponent } from './shared/components/kpi-card/kpi-card.component';
import { ChartPanelComponent } from './shared/components/chart-panel/chart-panel.component';
import { DataTableComponent } from './shared/components/data-table/data-table.component';
import { FilterBarComponent } from './shared/components/filter-bar/filter-bar.component';
import { StatusBannerComponent } from './shared/components/status-banner/status-banner.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { SalesComponent } from './pages/sales/sales.component';
import { ProductsComponent } from './pages/products/products.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { GeographyComponent } from './pages/geography/geography.component';
import { EmployeesComponent } from './pages/employees/employees.component';
import { DimensionsComponent } from './pages/dimensions/dimensions.component';

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    NavbarComponent,
    KpiCardComponent,
    ChartPanelComponent,
    DataTableComponent,
    FilterBarComponent,
    StatusBannerComponent,
    DashboardComponent,
    SalesComponent,
    ProductsComponent,
    CustomersComponent,
    GeographyComponent,
    EmployeesComponent,
    DimensionsComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
