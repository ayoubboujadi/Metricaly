import { Component, OnInit } from '@angular/core';
import { NavigationEnd, ActivatedRoute, Router } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {

  static readonly ROUTE_DATA_BREADCRUMB = 'breadcrumb';
  static readonly ROUTE_DATA_BREADCRUMB_HIDE = 'hideBreadcrumb';
  breadcrumbItems: BreadcrumbItem[];
  hide = false;

  constructor(private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.breadcrumbItems = this.createBreadcrumbs(this.activatedRoute.root);
        if (this.breadcrumbItems.length > 0) {
          this.breadcrumbItems[this.breadcrumbItems.length - 1].isUrl = false;
          this.hide = this.breadcrumbItems[this.breadcrumbItems.length - 1].hide;
        }

        this.breadcrumbItems.unshift({
          label: 'Home', isUrl: true, url: '/', hide: false
        });
      });
  }

  private createBreadcrumbs(route: ActivatedRoute, url: string = '', breadcrumbs: BreadcrumbItem[] = []): BreadcrumbItem[] {
    const children: ActivatedRoute[] = route.children;

    if (children.length === 0) {
      return breadcrumbs;
    }

    for (const child of children) {
      const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
      if (routeURL !== '') {
        url += `/${routeURL}`;
      }

      const hide = child.snapshot.data[BreadcrumbComponent.ROUTE_DATA_BREADCRUMB_HIDE];
      const label = child.snapshot.data[BreadcrumbComponent.ROUTE_DATA_BREADCRUMB];
      if (label !== null && label !== undefined) {
        breadcrumbs.push({ label, url, isUrl: true, hide });
      }


      return this.createBreadcrumbs(child, url, breadcrumbs);
    }
  }

}

export class BreadcrumbItem {
  label: string;
  url: string;
  isUrl = true;
  hide = false;
}
