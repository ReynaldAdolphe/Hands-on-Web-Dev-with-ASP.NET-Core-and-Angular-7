import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
//import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ModelModule } from "./models/model.module";
//import { MovieTableComponent } from "./structure/movieTable.component"
//import { CategoryFilterComponent } from "./structure/categoryFilter.component"
//import { MovieDetailComponent } from "./structure/movieDetail.component";
import { RoutingConfig } from "./app.routing";
import { StoreModule } from "./store/store.module";
import { MovieSelectionComponent } from "./store/movieSelection.component";
import { AdminModule } from "./admin/admin.module";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ModelModule,
    FormsModule,
    RoutingConfig, 
    StoreModule,
    AdminModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
