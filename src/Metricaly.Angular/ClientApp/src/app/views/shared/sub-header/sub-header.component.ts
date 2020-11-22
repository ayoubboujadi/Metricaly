import { Component, OnInit } from '@angular/core';
import { SubHeaderService } from './sub-header.service';

@Component({
  selector: 'app-sub-header',
  templateUrl: './sub-header.component.html',
  styleUrls: ['./sub-header.component.css']
})
export class SubHeaderComponent implements OnInit {

  title: string;

  constructor(private subHeaderService: SubHeaderService) {
  }

  ngOnInit(): void {
    this.subHeaderService.onSetTitle()
      .subscribe((data: { title: string }) => {
        this.title = data.title;
      });
  }

}
