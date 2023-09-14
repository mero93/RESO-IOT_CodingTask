import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Subject, filter, take, takeUntil } from 'rxjs';
import { IdentificationService } from '../services/identification.service';
import { Profile } from '../models/profile.model';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MSAL_GUARD_CONFIG, MsalGuardConfiguration, MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus, RedirectRequest } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, OnDestroy{
  private readonly _destroy = new Subject<void>();
  userLoggedIn = false
  profile: Profile
  profilePic: SafeResourceUrl

  constructor(@Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private msalBroadcastService: MsalBroadcastService,
    private authService: MsalService,
    private identificationService: IdentificationService,
    private domSanitizer: DomSanitizer) { }

  ngOnInit(): void {
    console.log('service initializing')
    this.msalBroadcastService.inProgress$.pipe(
      filter((interactionStatus:InteractionStatus) => 
      interactionStatus == InteractionStatus.None),
      takeUntil(this._destroy)
    ).subscribe(x => {
      this.userLoggedIn = this.authService.instance.getAllAccounts().length > 0
      this.identificationService.userLoggedIn.next(this.userLoggedIn)
    })

    this.getProfile()
  }

  ngOnDestroy(): void {
    this._destroy.next(undefined);
    this._destroy.complete();
  }

  login() {
    if (this.msalGuardConfig.authRequest)
    {
      this.authService.loginRedirect({...this.msalGuardConfig.authRequest} as RedirectRequest)
    }
    else
    {
      this.authService.loginRedirect()
    }
  }

  logout() {
    this.authService.logoutRedirect({postLogoutRedirectUri:environment.postLogoutUrl})
  }

  getProfile() {
    this.identificationService.getUserProfile()
    .pipe(take(1)).subscribe(response => {
      console.log('logging response:',response)
      this.profile = response
    })
  }
}
