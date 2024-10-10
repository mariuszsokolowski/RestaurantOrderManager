
<template >
  <div style="max-width:100vw">
    <snackbar
      v-if="snackbar"
      v-on:doneSnackbar="snackbar=false"
      :snackbarText="snackbarText"
      :snackbarColor="snackbarColor"
    />
    <v-toolbar color="deep-purple darken-3" dark style="width:100vw">
      <router-link to="/">
        <v-tooltip bottom>
          <v-btn
            icon
            v-show="this.cookiesAuth==this.cookiesAuth"
            slot="activator"
            style="width: 100%"
          >Menu
            <v-icon medium>fastfood</v-icon>
          </v-btn>
          <span>Check our menu</span>
        </v-tooltip>
      </router-link>

      <v-tooltip bottom>
        <v-btn
          icon
          @click="callWaiter"
          slot="activator"
          style="width: 100%"
          v-show="this.$cookies.isKey('authToken')"
        >Waiter
          <v-icon large class="bell" color="success">
            <!-- notification_important -->
            notifications_none
          </v-icon>
        </v-btn>
        <span>Call the waiter to your station</span>
      </v-tooltip>
      <v-spacer></v-spacer>

      <v-menu offset-y :nudge-width="100" v-show="this.$cookies.isKey('authToken')">
        <v-toolbar-title slot="activator" v-show="$cookies.get('authRole')!=='Client'">
            <v-btn icon style="width:100%">
                Manage
                <v-icon medium>build</v-icon>
            </v-btn>
        </v-toolbar-title>

        <v-list>
          <v-list-tile
            v-for="item in items"
            :key="item.route"
            v-show="$cookies.get('authRole')==item.role || $cookies.get('authRole')=='Administrator' "
          >
            <router-link :to="item.route">
              <v-list-tile-title v-text="item.title"></v-list-tile-title>
            </router-link>
          </v-list-tile>
        </v-list>
      </v-menu>

      <div v-show="this.$cookies.isKey('authToken')">
        <v-btn icon style="width:100%">
          <span v-show="$cookies.get('authRole')=='Client'">Cart</span>
          <v-badge right @click.native="openShopingCard" overlap color="success">
            <span slot="badge">{{ counter }}</span>
            <v-icon medium>shopping_cart</v-icon>
          </v-badge>
        </v-btn>
      </div>
      <router-link to="/Account/Login">
        <v-btn icon style="width:100%" v-show="!this.$cookies.isKey('authToken')">
          <v-icon medium>person</v-icon>
        </v-btn>
      </router-link>

      <v-btn icon v-show="this.$cookies.isKey('authToken')" @click="logout">
        <v-icon medium color="red">person</v-icon>
      </v-btn>
    </v-toolbar>

    <v-dialog
      v-model="shopingCardDialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-card>
        <v-toolbar dark color="primary">
          <v-toolbar-title>Cart</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items>
            <v-btn icon dark @click.native="shopingCardDialog = false">
              <v-icon>close</v-icon>
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <div v-show="counter>0">
          <v-divider></v-divider>

          <v-container fluid grid-list-md>
            <v-layout row wrap justify-space-between>
              <v-flex xs12 sm6 md8>
                <v-data-table
                  :headers="headers"
                  :items="shopingCard"
                  hide-actions
                  class="elevation-1"
                  no-data-text="Your cart is empty!"
                  d-flex
                >
                  <template slot="items" slot-scope="props">
                    <td>
                      <v-avatar size="6vw">
                        <img :src="'https://localhost:44389/api/Menu/GetImg/'+props.item.Img" alt>
                      </v-avatar>
                    </td>
                    <td>{{ props.item.Name }}</td>
                    <td>
                      <v-text-field
                        type="number"
                        min="1"
                        v-model.number="props.item.Quantity"
                        mask="###"
                        name="input-1"
                      ></v-text-field>
                    </td>
                    <td>{{ props.item.Price }}</td>
                    <td>{{ props.item.Price*props.item.Quantity }}</td>
                    <td class="justify-center">
                      <v-icon small color="red" @click="deleteItem(props.item)">delete</v-icon>
                    </td>
                  </template>
                  <v-alert
                    slot="no-results"
                    :value="true"
                    color="error"
                    icon="warning"
                  >No data found with key: .</v-alert>
                </v-data-table>
              </v-flex>

              <v-flex xs12 sm6 md3>
                <v-card class="card" d-flex style="width:100%;text-align:center">
                  <v-card-title primary-title>
                    <h3 class="text-md-center headline mb-0 font-weight-black">Total</h3>
                    <div style="height:100%;width:100%;background-color:green">
                      <v-divider></v-divider>
                    </div>
                  </v-card-title>

                  <h2
                    class="text-md-center headline mb-0 font-weight-light font-italic"
                  >To be paid: {{orderPrice}} zł</h2>
                  <div style="height:100%;width:100%;background-color:green">
                    <v-divider></v-divider>
                  </div>
                  <v-form ref="form" v-model="valid" lazy-validation>
                    <v-select
                      :items="ClientLocations"
                      v-model="ActualClient"
                      v-show="$cookies.get('authRole')!=='Client'"
                      label="Position"
                      outline
                      :rules="userRules"
                    ></v-select>
                  </v-form>

                  <v-card-actions style="align-items: center;justify-content: center;">
                    <v-btn flat color="green" @click.native="orderStepOne">Accept</v-btn>
                    <v-btn flat color="red" @click.native="cancleOrder">Close</v-btn>
                  </v-card-actions>
                </v-card>
              </v-flex>
            </v-layout>
          </v-container>
        </div>
      </v-card>
    </v-dialog>

    <v-layout row justify-center>
      <v-dialog v-model="dialog1" scrollable max-width="300px">
        <v-card>
          <v-card-title>Choose payment method</v-card-title>
          <v-divider></v-divider>
          <v-card-text style="height: 300px;">
            <v-radio-group v-model="paymentMethod" column>
              <v-radio label="Cash" v-bind:value="paymentMethodEnum.Cash"></v-radio>

              <v-radio label="Card" v-bind:value="paymentMethodEnum.Card"></v-radio>

              <v-radio label="Blik" v-bind:value="paymentMethodEnum.Blik"></v-radio>
            </v-radio-group>
          </v-card-text>
          <v-divider></v-divider>
          <v-card-actions>
            <v-btn color="blue darken-1" flat @click.native="order">Next</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-layout>
  </div>
</template>


<script>
import { eventBus } from "../index";
import Snackbar from "../Helpers/Snackbar.vue";

export default {
  components: {
    Snackbar
  },
  data: () => ({
    //  counter:0,
    url: "https://localhost:44389/api/Order/",
    urlNotification: "https://localhost:44389/api/Notification/",
    urlUser: "https://localhost:44389/api/User/",

    snackbar: false,
    ClientLocations: [],
    ActualClient: "",
    snackbarText: "",
    snackbarColor: "green",
    cookiesAuth: false,
    shopingCard: [],
    Order: [],
    tempOrder: {
      Id: 0,
      MenuId: 0,
      Quantity: 0,
      Price: 0
    },
    dialog: true,
    dialog1: false,
    paymentMethod: "",
    paymentMethodEnum: {
      Cash: "1",
      Card: "2",
      Blik: "3"
    },
    shopingCardDialog: false,
    items: [
      {
        title: "Menu",
        route: "/Menu",
        role: "Waiter"
      },
      {
        title: "Orders",
        route: "/Orders",
        role: "Cook"
      },
      {
        title: "Ready orders",
        route: "/Orders/Completed",
        role: "Waiter"
      },
      {
        title: "Users",
        route: "/Users",
        role: "Administrator"
      },
      {
        title: "Notification",
        route: "/Notification",
        role: "Waiter"
      },
      {
        title: "Reports",
        route: "/Raports",
        role: "Administrator"
      }
    ],
    headers: [
      {
        text: "",
        align: "left",
        value: "Avatar",
        sortable: false
      },
      { text: "Name", value: "Name" },
      { text: "Quantity", value: "Quantity" },
      { text: "Price", value: "Price" },
      { text: "Total line", value: "TotalLine" },
      { text: "", value: "" }
    ],

    //Rules
    valid: true,
    userRules: [v => !!v || "Choose location to your order."]
  }),

  methods: {
    enableSnackbar(text, color) {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },
    UpdateCounter() {
      var i;

      for (i = 0; i < this.shopingCard.length; i++) {
        //   this.counter+=parseFloat(this.shopingCard[i].Quantity);
      }
    },
    deleteItem(item) {
      var index = this.shopingCard.indexOf(item);
      if (confirm("Are you sure to delete item: " + item.Name + "?")) {
        this.shopingCard.splice(index, 1);
      }
    },

    orderStepOne() {
      if ($cookies.get("authRole") !== "Client") {
        if (!this.$refs.form.validate()) {
          return;
        }
      }
      this.dialog1 = true;
    },
    order() {
      if (this.paymentMethod == "") {
        alert("Choose payment method");
        return;
      }
      this.shopingCard[0].paymentMethod = this.paymentMethod;
      console.log("paymentMethod", this.shopingCard);

      this.dialog1 = false;

      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      var url = this.url;
      if (this.$cookies.get("authRole") !== "Client") {
        url += "ForClient";
        for (var i = 0; i < this.shopingCard.length; i++) {
          this.shopingCard[i].UserSign = this.ActualClient;
        }
        console.log(this.shopingCard);
      }

      this.axios
        .post(url, this.shopingCard, config)

        .then(response => {
          this.shopingCard = [];
          this.shopingCardDialog = false;
          this.enableSnackbar(response.data, "success");
        });
    },
    getClientLocation() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.axios
        .get(this.urlUser + "Clients", config)

        .then(response => {
          this.ClientLocations = response.data;
        });
    },
    cancleOrder() {
      if (confirm("Do you want to close order?")) {
        this.shopingCard = [];
      }
    },
    callWaiter() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.axios
        .post(this.urlNotification + "CallWaiter", null, config)

        .then(response => {
          this.shopingCardDialog = false;
          this.enableSnackbar(response.data, "success");
        });
    },
    logout() {
      var url = "https://localhost:44389/api/Account/Logout";
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.axios
        .post(url, null, config)

        .then(response => {
          this.$cookies.remove("authToken");
          this.$router.go();
        });
    },
    openShopingCard() {
      this.shopingCardDialog = true;
      if (this.$cookies.get("authToken") !== "Client") {
        this.getClientLocation();
      }
    }
  },

  created() {
    if (this.shopingCard.length <= 0 && this.$cookies.isKey("shopingCard")) {
      var sCard = JSON.parse(this.$cookies.get("shopingCard"));
      this.shopingCard = sCard;
      this.$cookies.set("shopingCard", this.shopingCardDialog, 1);
    } else {
    }
    eventBus.$on("shopingCardEdited", shopingcard => {
      if (this.shopingCard.find(x => x.MenuId == shopingcard.MenuId) != null) {
        this.shopingCard.find(x => x.MenuId == shopingcard.MenuId).Quantity +=
          shopingcard.Quantity;
      } else {
        this.shopingCard.push(JSON.parse(JSON.stringify(shopingcard)));
      }
      this.UpdateCounter();
    });
    eventBus.$on("cookiesAuthEdited", cookieauth => {
      this.cookiesAuth = this.$cookies.isKey("authToken");
    });
  },
  computed: {
    counter() {
      let sum = 0;
      let value = 0;
      var i;
      for (i = 0; i < this.shopingCard.length; i++) {
        if (this.shopingCard[i].Quantity == "") {
          this.shopingCard[i].Quantity = 1;
        }

        value = parseInt(this.shopingCard[i].Quantity);
        sum += value;
      }
      if (this.shopingCard.length > 0) {
        var test = JSON.parse(this.$cookies.get("shopingCard"));
        console.log("Cookies", this.$cookies.get("shopingCard"));
      }
      return sum;
    },

    orderPrice() {
      var sum = 0;
      var i;
      for (i = 0; i < this.shopingCard.length; i++) {
        sum += parseFloat(
          this.shopingCard[i].Quantity * this.shopingCard[i].Price
        );
      }
      return sum;
    }
  },
  watch: {
    shopingCard: {
      handler(val) {
        var i = 0;
        console.log(val);
        for (; i < val.length; i++) {
          if (
            !val[i].Quantity ||
            val[i].Quantity == 1 ||
            val[i].Quantity == 0
          ) {
            // val[i].Quantity=1;
          }
        }

        this.$cookies.set(
          "shopingCard",
          JSON.stringify(this.shopingCard),
          60 * 10
        );
      },
      deep: true
    }
  }
};
</script>
<style scope>
.card {
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);
  transition: 0.3s;
  width: 40%;
}

.card:hover {
  box-shadow: 0 8px 16px 0 rgba(0, 0, 0, 0.2);
}
h2,
h1,
h3 {
  width: 100%;
}
.v-card__title--primary {
  background-color: #f5f5f5;
  margin-bottom: 5vh;
}
.v-card__actions {
  margin-top: 5vh;
}
</style>