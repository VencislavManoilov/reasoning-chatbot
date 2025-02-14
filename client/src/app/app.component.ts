import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Router, NavigationEnd  } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client';

  constructor(private router: Router) {}

  async checkSession(): Promise<void> {
    const token = localStorage.getItem('token');
    if (!token) {
      return;
    }

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd) // Wait for navigation to finish
    ).subscribe((event: NavigationEnd) => {
      // Allow any /chat/* path or /profile
      if(!event.url.startsWith('/chat') && event.url !== '/profile') {
        this.router.navigate(['/chat']);
      }
    });
  }

  ngOnInit() {
    this.checkSession();
  }
}