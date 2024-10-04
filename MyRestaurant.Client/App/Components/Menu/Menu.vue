<template >
  <v-container grid-list-md text-xs-center>
    <snackbar
      v-if="snackbar"
      v-on:doneSnackbar="snackbar=false"
      :snackbarText="snackbarText"
      :snackbarColor="snackbarColor"
    />
    <v-layout row wrap>
      <v-dialog v-model="dialog" persistent max-width="300px" v-if="dialog">
        <v-form ref="form" v-model="valid" lazy-validation>
          <v-card>
            <v-card-title class="grey lighten-4" style="display:flex">
              <v-avatar :tile="false" size="9vh" color="grey lighten-4">
                <img
                  :src="'https://localhost:44389/api/Menu/GetImg/' + menu.find(x=>x.MenuId==DialogItem).Image"
                  alt
                >
              </v-avatar>

              <span
                class="headline"
                style="margin: auto;"
              >{{menu.find(x=>x.MenuId==DialogItem).Name}}</span>
            </v-card-title>

            <v-card-text>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs12 v-show="this.$cookies.isKey('authToken')">
                    <v-text-field
                      v-model.number="itemCounter"
                      :rules="InputNumberRules"
                      type="number"
                      label="Ilość"
                      step="1"
                      required
                      style="margin-right:auto"
                    ></v-text-field>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" flat @click.native="dismissDialog">Anuluj</v-btn>
              <v-btn
                color="blue darken-1"
                flat
                @click.native="AddToShoppingCard"
                v-show="this.$cookies.isKey('authToken')"
              >Dodaj do koszyka</v-btn>
            </v-card-actions>
          </v-card>
        </v-form>
      </v-dialog>

      <v-card
        class="elevation-16 mx-auto"
        width="300"
        v-show="rateCollection.length>0 && rateItem.length<=0"
      >
        <v-card-title class="headline" primary-title>Oceń nasze dania</v-card-title>
        <v-card-text>Pomóż nam i oceń zamówione danie</v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="justify-space-between">
          <v-btn flat @click="cancelAllRate">Nie dziękuję</v-btn>
          <v-btn color="primary" flat @click="startRating">Oceń</v-btn>
        </v-card-actions>
      </v-card>

      <v-card
        class="elevation-16 mx-auto"
        width="300"
        v-show="rateCollection.length<=0 && rateItem.length>0"
        v-for="item in firstRateItem"
        :key="item.Name"
      >
        <v-card-title class="headline text-center" primary-title>
          <div style="margin: auto">
            <v-avatar color="grey lighten-4">
              <img
                style="float:left"
                :src="'https://localhost:44389/api/Menu/GetImg/'+item.OrderLine.Menu.Image"
              >
            </v-avatar>
            <span>{{ item.OrderLine.Menu.Name }}</span>
          </div>
        </v-card-title>
        <v-card-text>
          <span>Pomóż nam i oceń zamówione danie</span>

          <v-rating
            v-model="postRateItem.rating"
            color="yellow darken-3"
            background-color="grey darken-1"
            empty-icon="$vuetify.icons.ratingFull"
            half-increments
            hover
          ></v-rating>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="justify-space-between">
          <v-btn flat @click="cancelRate(item)">Nie dziękuję</v-btn>
          <v-btn color="primary" flat @click="rate(item)">Oceń</v-btn>
        </v-card-actions>
      </v-card>

      <v-card
        class="elevation-16 mx-auto"
        width="300"
        v-show="rateWaiterCollection.length>0 && rateWaiterItem.length<=0"
      >
        <v-card-title class="headline" primary-title>Oceń naszych pracowników</v-card-title>
        <v-card-text>Pomóż nam i oceń naszych kelenrów</v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="justify-space-between">
          <v-btn flat @click="cancelAllWaiterRate">Nie dziękuję</v-btn>
          <v-btn color="primary" flat @click="startWaiterRating">Oceń</v-btn>
        </v-card-actions>
      </v-card>

      <v-card
        class="elevation-16 mx-auto"
        width="300"
        v-show="rateWaiterCollection.length<=0 && rateWaiterItem.length>0"
        v-for="item in firstWaiterRateItem"
        :key="item.WaiterRateID"
      >
        <v-card-title class="headline" primary-title>
          <span style="margin: auto">{{ item.Waiter.Name }}</span>
        </v-card-title>
        <v-card-text>Oceń naszego kelnera
          <v-rating
            v-model="postRateWaiterItem.rating"
            color="yellow darken-3"
            background-color="grey darken-1"
            empty-icon="$vuetify.icons.ratingFull"
            half-increments
            hover
          ></v-rating>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="justify-space-between">
          <v-btn flat @click="cancelWaiterRate(item)">Nie dziękuję</v-btn>
          <v-btn color="primary" flat @click="rateWaiter(item)">Oceń</v-btn>
        </v-card-actions>
      </v-card>

      <v-flex v-for="item in menu" :key="`${item.MenuId}`" xs12 sm6 md4>
        <v-hover style="margin-top:1vw; ">
          <v-card
            :class="`elevation-${hover ? 6 : 22}`"
            slot-scope="{ hover }"
            color="grey lighten-4"
          >
            <v-img
              :aspect-ratio="16/9"
              :src="'https://localhost:44389/api/Menu/GetImg/'+item.Image"
            >
              <v-chip
                color="orange"
                text-color="white"
                v-model="item.Promotion"
                style="float:left"
                v-if="!item.Inactive"
              >Promocja!
                <v-icon right>star</v-icon>
              </v-chip>

              <v-expand-transition>
                <div
                  v-if="hover && !item.Inactive"
                  class="d-flex transition-fast-in-fast-out orange darken-2 v-card--reveal display-3 white--text"
                  style="height: 100%;"
                >
                  <span v-if="!item.Inactive">{{item.Price}} PLN</span>
                </div>
                <div
                  v-if="item.Inactive"
                  class="d-flex transition-fast-in-fast-out darken-2 v-card--reveal display-3 white--text"
                  style="height: 100%;background-color:grey"
                >
                  <span v-if="item.Inactive" style="font-size:3vh">NIEDOSTĘPNE</span>
                </div>
              </v-expand-transition>
            </v-img>
            <div style="height:5vh">
              <v-rating
                v-show="item.Rating>0"
                readonly
                v-model="item.Rating"
                color="yellow darken-3"
                background-color="grey darken-1"
                empty-icon="$vuetify.icons.ratingFull"
                half-increments
                hover
              ></v-rating>
              <v-card-text v-show="item.Rating<=0" color="success">Bądź pierwszy i oceń danie</v-card-text>
            </div>
            <v-card-text class="pt-4" style="position: relative;">
              <v-btn
                v-show="item.Inactive===false"
                absolute
                color="orange"
                class="white--text"
                fab
                medium
                right
                top
                @click="OpenDialog(item.MenuId)"
                style="margin-top:1vh; margin-left:1vh"
              >
                <v-icon>shopping_cart</v-icon>
              </v-btn>

              <h3 class="display-1 font-weight-light orange--text mb-2">{{item.Name}}</h3>
              <div class="font-weight-light title mb-2"></div>
            </v-card-text>
          </v-card>
        </v-hover>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<style>
.v-card--reveal {
  align-items: center;
  bottom: 0;
  justify-content: center;
  opacity: 0.6;
  position: absolute;
  width: 100%;
}
.pt-4 {
  background-color: #eeeeee;
}
</style>

<script>
import { eventBus } from "../../index";

import Snackbar from "../../Helpers/Snackbar.vue";

export default {
  components: {
    Snackbar
  },
  data: function() {
    return {
      connection: null,
      menu: [],
      rateCollection: [],
      rateWaiterCollection: [],
      valid: true,
      showDescription: false,
      snackbar: false,
      snackbarText: "",
      snackbarColor: "",
      shopingCard: {
        MenuId: 0,
        Price: 0,
        Quantity: 0,
        Name: "",
        Img: ""
      },
      rateItem: [],
      rateWaiterItem: [],
      postRateItem: {
        rating: 4.0,
        RateID: 0
      },
      postRateWaiterItem: {
        rating: 4.0,
        WaiterRateID: 0
      },

      dialog: false,
      itemCounter: 1,
      DialogItem: "",
      InputNumberRules: [
        v => !!v || "Wprowadź ilość",
        v => v >= 0 || "Ilość musi być większa od",
        v => v % 1 == 0 || "Nie można kupywać w ulamku"
      ]
    };
  },

  methods: {
    enableSnackbar(text, color) {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },
    fetchData() {
      this.axios
        .get("https://localhost:44389/api/Menu")
        .then(response => (this.menu = response.data))
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
      //this.loadClient();
    },

    OpenDialog(id) {
      this.DialogItem = id;

      this.dialog = true;
      console.log("menu", this.menu);
    },
    fetchRateCollection() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      var tempdata;
      this.axios
        .get("https://localhost:44389/api/Rate", config)
        .then(response => (this.rateCollection = response.data))
        .catch(function(error) {
          console.error(error);
        });
    },

    fetchWaiterRateCollection() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      var tempdata;
      this.axios
        .get("https://localhost:44389/api/WaiterRate", config)
        .then(response => (this.rateWaiterCollection = response.data))
        .catch(function(error) {
          console.error(error);
        });
    },
    startRating() {
      this.rateItem = this.rateCollection;
      this.rateCollection = [];
    },
    startWaiterRating() {
      this.rateWaiterItem = this.rateWaiterCollection;
      this.rateWaiterCollection = [];
    },
    cancelAllRate() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      console.log("token", config.headers);
      this.axios
        .post("https://localhost:44389/api/Rate/CancelAll", null, config)
        .then(response => (this.rateCollection = []))
        .catch(function(error) {
          console.error(error);
        });
    },
    cancelRate(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .post(
          "https://localhost:44389/api/Rate/Cancel",
          { RateID: item.RateID },
          config
        )
        .then(response => this.rateItem.splice(item, 1))
        .catch(function(error) {
          console.error(error);
        });
    },
    cancelWaiterRate(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .post(
          "https://localhost:44389/api/WaiterRate/Cancel",
          { WaiterRateID: item.WaiterRateID },
          config
        )
        .then(response => this.rateWaiterItem.splice(item, 1))
        .catch(function(error) {
          console.error(error);
        });
    },

    cancelAllWaiterRate(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .post(
          "https://localhost:44389/api/WaiterRate/CancelAll",
          { WaiterRateID: item.WaiterRateID },
          config
        )
        .then(response => this.rateWaiterItem.splice(item, 1))
        .catch(function(error) {
          console.error(error);
        });
    },

    rate(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.postRateItem.RateID = item.RateID;
      this.axios
        .post(
          "https://localhost:44389/api/Rate/RateMenu",
          this.postRateItem,
          config
        )
        .then(response => {
          this.rateItem.splice(item, 1);
          this.enableSnackbar("Dziękujemy za ocenę.", "success");
        })
        .catch(function(error) {
          console.error(error);
        });
    },

    rateWaiter(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.postRateWaiterItem.WaiterRateID = item.WaiterRateID;
      this.axios
        .post(
          "https://localhost:44389/api/WaiterRate/Rate",
          this.postRateWaiterItem,
          config
        )
        .then(response => {
          this.rateWaiterItem.splice(item, 1);
          this.enableSnackbar("Dziękujemy za ocenę.", "success");

        })
        .catch(function(error) {
          console.error(error);
        });
    },
    AddToShoppingCard() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.shopingCard.Quantity = parseInt(this.itemCounter);
      this.shopingCard.MenuId = this.DialogItem;

      this.shopingCard.Price = this.menu.find(
        x => x.MenuId == this.DialogItem
      ).Price;
      this.shopingCard.Name = this.menu.find(
        x => x.MenuId == this.DialogItem
      ).Name;
      this.shopingCard.Img = this.menu.find(
        x => x.MenuId == this.DialogItem
      ).Image;

      this.itemCounter = 1;
      this.dialog = false;

      eventBus.changeShoppingCard(this.shopingCard);

      this.enableSnackbar("Dodano do koszyka.", "success");
    },
    dismissDialog(event) {
      this.dialog = false;
      this.itemCounter = 1;
    }

    //
    // someMethod() {
    //   this.$socket.invoke('Send','test')
    //     .then(response => {
    //     console.log("Signalr Send running!")
    //     })
    // }
  },
  created: function() {
    this.connection = new this.$signalR.HubConnectionBuilder()
      .withUrl("https://localhost:44389/rateHub")
      .configureLogging(this.$signalR.LogLevel.Error)
      .build();
    this.fetchWaiterRateCollection();
    this.fetchRateCollection();
  },
  computed: {
    firstRateItem() {
      return this.rateItem.slice(0, 1);
    },
    firstWaiterRateItem() {
      return this.rateWaiterItem.slice(0, 1);
    }
  },
  mounted() {
    var thisContext = this;
    this.fetchData();
    //var connection = new signalR.HubConnection("https://localhost:44389/menuHub");
    this.connection.on("MenuRate", function() {
      thisContext.fetchRateCollection();
      console.log("MenuRate");
    });
    this.connection.on("WaiterRate", function() {
      thisContext.fetchWaiterRateCollection();
      console.log("WaiterHub");
    });
    this.connection.start().then(function() {
      console.log("SignalR started");
    });
  }
};
</script>
