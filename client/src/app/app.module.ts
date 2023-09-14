import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MsalGuard, MsalInterceptor, MsalModule, MsalRedirectComponent } from '@azure/msal-angular';
import { InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { HomeComponent } from './home/home.component';
import { NavComponent } from './nav/nav.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule} from '@angular/material/icon';
import { IdentificationService } from './services/identification.service';
import { GoogleMapsModule } from '@angular/google-maps';
import {MatCardModule} from '@angular/material/card';
import {MatSliderModule} from '@angular/material/slider';
import {MatTableModule} from '@angular/material/table';
import {MatDialogModule} from '@angular/material/dialog';
import {ClipboardModule} from '@angular/cdk/clipboard';

import { FormsModule } from '@angular/forms';
import { ReportPageComponent } from './report-page/report-page.component';



const isIE = window.navigator.userAgent.indexOf('MSIE') > -1
  || window.navigator.userAgent.indexOf('Trident/') > -1

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavComponent,
    ReportPageComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatButtonModule,
    GoogleMapsModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule,
    MatSliderModule,
    MatTableModule,
    ClipboardModule,
    MsalModule.forRoot(new PublicClientApplication
      (
        {
          auth:{
            clientId:'f1149c8a-b761-4d94-9171-18f7f52927d5',
            authority:'https://login.microsoftonline.com/f554122d-21b3-41e8-bdae-53b5f2a44d10',
            redirectUri:"http://localhost:4200",
          },
          cache:
          {
            cacheLocation:'localStorage',
            storeAuthStateInCookie: isIE
          }
        }
      ),
      {
        interactionType:InteractionType.Redirect,
        authRequest:{
          scopes:['user.read', 'profile']
        }
      },
      {
        interactionType:InteractionType.Redirect,
        protectedResourceMap: new Map(
          [
            ['https://graph.microsoft.com/v1.0/me', ['user.Read']],
            ['localhost', ['api://020b0ad8-48c4-46c2-a2d5-10178d9b9c4d/api.scope']]
          ]
        )
      },
      ),
  ],
  providers: [{
    provide:HTTP_INTERCEPTORS,
    useClass:MsalInterceptor,
    multi:true
  }, MsalGuard, IdentificationService],
  bootstrap: [AppComponent, MsalRedirectComponent]
})
export class AppModule { }
