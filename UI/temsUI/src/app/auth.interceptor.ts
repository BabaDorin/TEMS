import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private router: Router) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let token = localStorage.getItem('token');
        if (token == null) token = 'signed out';

        const clonedReq = req.clone({
            headers: req.headers.set('Authorization', 'bearer ' + localStorage.getItem('token'))
        });
        return next.handle(clonedReq).pipe(
            tap(
                succ => {
                },
                err => {
                    if (err.status == 401) {
                        localStorage.removeItem('token');
                        this.router.navigateByUrl('/auth/login');
                    }
                }
            )
        )
    }
}