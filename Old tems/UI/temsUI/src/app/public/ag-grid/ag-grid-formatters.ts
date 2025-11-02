import { DatePipe } from "@angular/common";

let pipe = new DatePipe('en-US');
export function dateFormatter(params) {
  return pipe.transform(params.value, 'short');
}