import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  myRes: any;
  searchCode: string = "31126753992";

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  search() {
    console.log(this.searchCode);

     this.http.get<any[]>(this.baseUrl + 'search?code=' +this.searchCode).subscribe(result => {
       this.myRes = result;
       console.log(this.myRes);
     }, error => console.error(error));
  }
}
