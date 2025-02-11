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
        const request = await axios.get('http://localhost:5010/api/auth/profile', {withCredentials: true});

        if(request.status === 200) {
            this.user = request.data;
        } else {
            localStorage.removeItem('sessionId');
        }
    }

    ngOnInit(): void {
        
    }
}