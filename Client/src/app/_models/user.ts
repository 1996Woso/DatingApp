export interface User{
    username: string;
    token: string;
}

// export type User ={
//     username: string;
//     token: string;
// }
let data: number | string = 5;
data = "5";

interface Car {
    color: string,
    model: string,
    topSpeed?: number
}
const car1: Car = {
    color: "blue",
    model: "BMW"
}

const car2: Car  = {
    color: "red",
    model:  "Mercedes",
    topSpeed: 100
}

const multiply = (x: number,y: number) => {
     return 't';
}