import Vue from "vue";
import VueRouter from "vue-router";
import App from "./App.vue";
import Main from "./Components/Menu/Menu.vue";
import Menu from "./Components/Menu/Index.vue";
import User from "./Components/Users/Index.vue";
import MenuCreate from "./Components/Menu/Create.vue";
import Order from "./Components/Orders/Index.vue";
import OrderCompleted from "./Components/Orders/Completed.vue";
import AccountLogin from "./Components/Account/Login.vue";
import Notification from './Components/Notification/Index.vue'
import Raport from './Components/Raport/Index.vue'
import Vuetify from "vuetify";
import "vuetify/dist/vuetify.css";
import VueAxios from "vue-axios";
import axios from "axios";
import moment from "moment";
import ChartJS from 'vue-chartjs';
//import VueSignalR from '@latelier/vue-signalr'
// import jquery from 'jquery';


import VueCookies from "vue-cookies";

const routes = [
  { path: "/", component: Main },
  { path: "/Menu", component: Menu },
  { path: "/Menu/Create", component: MenuCreate },
  { path: "/Users", component: User },
  { path: "/Orders", component: Order },
  { path: "/Orders/Completed", component: OrderCompleted },
  { path: "/Account/Login", component: AccountLogin },
  { path: "/Notification", component: Notification },
  { path: "/Raports", component: Raport }
];

Vue.use(VueAxios, axios);
Vue.use(Vuetify);
Vue.config.productionTip = false;
Vue.use(VueRouter);
Vue.use(VueCookies);
Vue.use(moment);
Vue.use(ChartJS);

var signalR = require('../wwwroot/lib/signalr/signalr.min.js');
Vue.prototype.$signalR = signalR;
//Vue.use(VueSignalR,'https://localhost:44389/menuHub');
// Vue.use(jquery);

//Vue.prototype.$signalR = signalr;


Vue.filter("formatDate", function(value) {
  if (value) {
    return moment(String(value)).format("MM/DD/YYYY HH:mm");
  }
});

export const eventBus = new Vue({
  methods: {
    changeShoppingCard(counter) {
      this.$emit("shopingCardEdited", counter);
    },
    changeCookiesAuth()
    {
      this.$emit("cookiesAuthEdited");
    },
    created() {
      this.$emit("shopingCardEdited", 0);
    }
  }
});

const router = new VueRouter({
  routes,
  mode: "history"
});

new Vue({
  el: "#app",
  render: h => h(App),

  router
});
