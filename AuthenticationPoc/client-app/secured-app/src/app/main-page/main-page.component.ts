import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit, OnDestroy {

  title = 'secured-app';
  private subscriptions: Subscription[] = [];
  disableMedical:boolean;
  disableAncillary:boolean;
  disableHospital:boolean;
  events: string[];

  constructor(private jwtHelper: JwtHelperService, private http: HttpClient) {
  }
  
  ngOnInit(): void {
    this.disableMedical = false;
    this.disableAncillary = false;
    this.disableHospital = false;
    this.events = [];
  }
  login(event: any) {
    const url = "https://localhost:44316/api/v1/authenticate";
    if(!url) {
      this.writeEvent('fetching token');
      return;
    }
    const options = {
      params: {responseType:'json'},
      withCredentials: true
    }
    let sub = this.http.get(`${url}`, options).subscribe((response: any) => {
      localStorage.setItem('access_token', response.token);
      this.writeEvent(JSON.stringify(this.jwtHelper.decodeToken()));
      this.updateUI(this.jwtHelper.decodeToken());
    });
    this.subscriptions.push(sub);
  }
  logoff(event: any) {
    localStorage.removeItem('access_token');
    this.writeEvent('removing token from store');
  }
  updateUI(token:any) {
    // using private claims from JWT token
    this.disableAncillary = !(token.hasOwnProperty('IsAncillary') && token.IsAncillary === "True");
    this.disableMedical = !(token.hasOwnProperty('IsMedical') && token.IsMedical  === "True");
    this.disableHospital = !(token.hasOwnProperty('IsHospital') && token.IsHospital  === "True");
  }
  echo(event:any){
    this.writeEvent(event);
  }
  pingMed($event){
    this.get('https://localhost:44317/api/v1/medical');
  }
  pingHos($event){
    this.get('https://localhost:44317/api/v1/hospital');
  }
  pingAnc($event){
    this.get('https://localhost:44317/api/v1/ancillary');
  }
  private writeEvent(event){
    this.events.push(event);
  }
  private get(url: string){
    const options = {
      params: {responseType:'json'},
      withCredentials: true
    }
    this.http.get(url, options).subscribe(response=>this.writeEvent(JSON.stringify(response)), error => this.writeEvent(error.message));
  }
  clearEvents(event){
    this.events.length = 0;
  }
  ngOnDestroy(): void {
    this.subscriptions.forEach(s=>s.unsubscribe());
  }

}
