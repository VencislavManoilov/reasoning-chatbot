import { Component, SecurityContext, AfterViewChecked } from '@angular/core';
import axios from 'axios';
import { SidebarComponent } from "./sidebar/sidebar.component";
import { Router, ActivatedRoute } from '@angular/router';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MarkdownModule, MarkdownService, SECURITY_CONTEXT } from 'ngx-markdown';
import { HighlightModule, HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import hljs from 'highlight.js/lib/core';
import hljsLanguages from 'highlight.js/lib/common';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [SidebarComponent, NgFor, NgIf, NgClass, FormsModule, MarkdownModule, HighlightModule],
  providers: [
    MarkdownService,
    { provide: SECURITY_CONTEXT, useValue: SecurityContext.HTML },
    {
      provide: HIGHLIGHT_OPTIONS,
      useValue: {
        coreLibraryLoader: () => import('highlight.js/lib/core'),
        languages: { hljsLanguages },
        themePath: './chat.component.css'
      }
    }
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements AfterViewChecked {
  title = 'chat';
  chats: any[] = [];
  chatId: any;
  chat: any = [];
  messageContent: string = '';
  assistantMessage: { role: string, content: string } = { role: '', content: '' };

  constructor(private router: Router, private route: ActivatedRoute) {}

  async GetChat(id: number): Promise<void> {
    try {
      const response = await axios.get(environment.apiUrl+"/api/chat/view", {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        params: {
          ChatId: id
        }
      });

      this.chat = response.data.messages;
    } catch(error) {
      alert('Error getting chat');
      this.router.navigate(['/chat']);
    }
  }

  async GetChats(): Promise<void> {
    try {
      const response = await axios.get(environment.apiUrl+"/api/chat/list", {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      });

      this.chats = response.data;
    } catch(error) {
      try {
        await axios.get(environment.apiUrl+"/api/auth/profile", {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });

        alert('Error getting chat history');
      } catch(error) {
        localStorage.removeItem('token');
        window.location.href = '/';
      }
    }
  }

  async sendMessage(): Promise<void> {
    if (!this.messageContent.trim()) return;
  
    try {
      if(this.chat.length === 0) {
        this.chat = [{ role: "user", content: this.messageContent }];
      } else {
        this.chat.push({ role: "user", content: this.messageContent });
      }
  
      const params = new URLSearchParams();
      if(this.chatId) {
        params.append('ChatId', this.chatId);
      }
      params.append('Message', this.messageContent);
      this.messageContent = '';
      const textarea = document.querySelector('textarea');
      if (textarea) {
        textarea.blur();
        textarea.disabled = true;
      }
  
      const response = await fetch(environment.apiUrl+"/api/chat/send", {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: params
      });
  
      if (!response.body) {
        throw new Error('Response body is null');
      }

      const reader = response.body.getReader();
      const decoder = new TextDecoder('utf-8');
      this.assistantMessage = { role: "assistant", content: "" };
      
      while (true) {
        const { done, value } = await reader.read();
        if (done) break;
  
        this.assistantMessage.content += decoder.decode(value, { stream: true });
        this.updateChatView();
      }

      this.chat.push(this.assistantMessage);
      this.assistantMessage = { role: '', content: '' };

      const chatIdHeader = response.headers.get("Chat-Id");
      if (!this.chatId && chatIdHeader) {
        this.chatId = parseInt(chatIdHeader);
      }

      if(textarea) {
        textarea.disabled = false;
        textarea.focus();
      }
  
      this.updateChatView();
    } catch (error) {
      console.log(error);
      alert('Error sending message');
    }
  }

  updateChatView(): void {
    setTimeout(() => {
      const chatContainer = document.querySelector('#chat-messages');
      if (chatContainer) {
        chatContainer.scrollTop = chatContainer.scrollHeight;
      }
    }, 100);
  }

  adjustTextareaHeight(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = Math.min(textarea.scrollHeight, 5 * 24) + 'px';
  }

  openProfile(event: Event) {
    event.preventDefault();
    this.router.navigate(['/profile']);
  }

  ngOnInit() {
    this.GetChats();

    // Listen for changes in the chat ID
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.chatId = id;
        this.GetChat(parseInt(id));
      } else {
        this.chat = [];
        this.chatId = null;
      }
    });
  }

  ngAfterViewChecked() {
    this.highlightCode();
  }

  highlightCode() {
    const codeBlocks = document.querySelectorAll('pre code');
    codeBlocks.forEach((block) => {
      if (!(block as HTMLElement).dataset['highlighted']) {
        hljs.highlightElement(block as HTMLElement);
        (block as HTMLElement).dataset['highlighted'] = 'true';
      }
    });
  }
}
