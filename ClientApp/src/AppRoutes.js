import { Counter } from "./components/Counter";
import { Home } from "./components/Home";
import {Decrementer} from "./components/Decrementer";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/decrementer',
    element: <Decrementer />
  }
];

export default AppRoutes;
