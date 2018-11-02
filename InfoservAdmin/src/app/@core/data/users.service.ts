
import { of as observableOf,  Observable } from 'rxjs';
import { Injectable } from '@angular/core';


let counter = 0;

@Injectable()
export class UserService {

  private users = {
    nick: { name: 'Nick Jones' },
    eva: { name: 'Eva Moor' },
    jack: { name: 'Jack Williams' },
    lee: { name: 'Lee Wong' },
    alan: { name: 'Alan Thompson' },
    kate: { name: 'Kate Martinez' },
  };

  private userArray: any[];

  constructor() {
    // this.userArray = Object.values(this.users);
  }

  getUsers(): Observable<any> {
    return observableOf(this.users);
  }

  getUserArray(): Observable<any[]> {
    return observableOf(this.userArray);
  }

  getUser(): Observable<any> {
    counter = (counter + 1) % this.userArray.length;
    return observableOf(this.userArray[counter]);
  }
}
