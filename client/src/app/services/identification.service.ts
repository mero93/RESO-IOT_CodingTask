import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subject, filter, map, takeUntil } from 'rxjs';
import { MSAL_GUARD_CONFIG, MsalGuardConfiguration, MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus, RedirectRequest } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';
import { Profile } from '../models/profile.model';

const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me/';

@Injectable({
  providedIn: 'root'
})
export class IdentificationService {
  userLoggedIn:Subject<boolean> = new Subject<boolean>();

  constructor(
    private httpClient: HttpClient) { }

  getUserProfile()
  {
    return this.httpClient.get<Profile>(GRAPH_ENDPOINT);
  }
}
