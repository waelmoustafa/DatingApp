import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(private auth: AuthService) { }

  ngOnInit() {
  }
  login(model) {
    this.auth.login(this.model).subscribe(
      res => console.log('Logged In Successfully'),
      err => console.log('Failed to Logg in')
    );
  }
  LoggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  Logout() {
    localStorage.removeItem('token');
    console.log('Looged out');
  }
}
