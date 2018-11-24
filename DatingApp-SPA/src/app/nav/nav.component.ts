import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/Alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  model: any = {};
  userName: string;
  constructor(public auth: AuthService,
    private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
  }
  login(model) {
    this.auth.login(this.model).subscribe(
      res => {
        this.userName = this.auth.userName;
        this.alertify.success('Logged In Successfully') ;
      },
      err => {
        console.log(err);
        this.alertify.error(err);
      },
      () => {
        this.router.navigate(['members']);
      }
    );
  }
  LoggedIn() {
    return this.auth.loggedIn();
  }

  Logout() {
    localStorage.removeItem('token');
    this.alertify.message('Logged out');
    this.router.navigate(['home']);
  }
}
