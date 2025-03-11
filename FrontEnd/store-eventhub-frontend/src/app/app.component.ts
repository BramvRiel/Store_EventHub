import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { StoreEventHubPost } from './store-event-hub-post';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'store-eventhub-frontend';
  eventForm: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.eventForm = this.fb.group({
      productNumber: ['', [Validators.required, Validators.pattern(/^PROD\d{3}[A-Z]$/)]],
      customerNumber: ['', [Validators.required, Validators.pattern(/^\d{4}$/)]],
      eventDateTime: ['', Validators.required],
      eventBody: ['', [Validators.required, this.jsonValidator]]
    });
  }

  jsonValidator(control: any) {
    try {
      JSON.parse(control.value);
      return null;
    } catch (e) {
      return { invalidJson: true };
    }
  }

  fillPreset(type: string) {
    const presets: {[id: string]: StoreEventHubPost;} = {
      ordered: {
        productNumber: 'PROD100A',
        customerNumber: '1001',
        eventDateTime: new Date(2025,3,11,12,23).toISOString().slice(0, 16),
        eventBody: JSON.stringify({ event: 'Product Besteld', status: 'ordered' }, null, 2)
      },
      packed: {
        productNumber: 'PROD100A',
        customerNumber: '1001',
        eventDateTime: new Date(2025,3,11,12,24).toISOString().slice(0, 16),
        eventBody: JSON.stringify({ event: 'Product Ingepakt', status: 'packed' }, null, 2)
      },
      shipped: {
        productNumber: 'PROD100A',
        customerNumber: '1001',
        eventDateTime: new Date(2025,3,11,12,25).toISOString().slice(0, 16),
        eventBody: JSON.stringify({ event: 'Product Verzonden', status: 'shipped' }, null, 2)
      }
    };
    
    if (presets[type]) {
      this.eventForm.patchValue(presets[type]);
    }
  }

  submitEvent() {
    if (this.eventForm.valid) {
      const eventData = this.eventForm.value;
      console.log('Event Submitted:', eventData);
      // Hier kan de eventdata naar een service worden verstuurd
      
      this.http.post<StoreEventHubPost>('http://localhost:5055/process-event', eventData,{headers: {'Access-Control-Allow-Origin':'*'}}).subscribe(event => {console.log('have done something');})
    }
  }
}
