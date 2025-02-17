import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import axios from 'axios';
import { environment } from '../../environments/environment';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.css']
})

export class ProfileComponent implements OnInit {
    user: any;

    constructor(private router: Router) {}
    
    async getProfile() {
        try {
            const request = await axios.get(environment.apiUrl+"/api/auth/profile", {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                }
            });
            
            this.user = request.data;
        } catch(error) {
            localStorage.removeItem('token');
            this.router.navigate(['/']);
        }
    }

    openChat() {
        this.router.navigate(['/chat']);
    }

    ngOnInit(): void {
        this.getProfile();
    }
}