import Main from './Components/Menu/Menu.vue'
import Menu from './Components/Menu/Index.vue'
import MenuCreate from './Components/Menu/Create.vue'
import User from './Components/Users/Index.vue'
import Order from './Components/Orders/Index.vue'

const routes = [
    {path: '/', component: Main  },
     {path: '/Menu', component: Menu  },
     {path: '/Menu/Create', component: MenuCreate  },
     {path: '/Users', component: User  },
     {path: '/Orders', component: Order  }
  ]
