import { Component, OnInit, Input } from '@angular/core';
import axios from 'axios';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.css']
})

export class ProfileComponent implements OnInit {
    user: any;
    @Input() data: any;

    constructor() { 
        if(this.data) {
            this.user = this.data;
        } else {
            this.getProfile();
        }
    }
    
    async getProfile() {
        try {
            const request = await axios.get('http://localhost:5010/api/auth/profile', {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`
                }
            });
            
            this.user = request.data;
        } catch(error) {
            localStorage.removeItem('token');
            window.location.href = '/';
        }
    }

    ngOnInit(): void {
        
    }
}