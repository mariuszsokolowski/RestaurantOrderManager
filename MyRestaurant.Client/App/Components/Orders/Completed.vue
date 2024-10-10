 <template>
  <div>
    <snackbar
      v-if="snackbar"
      v-on:doneSnackbar="snackbar=false"
      :snackbarText="snackbarText"
      :snackbarColor="snackbarColor"
    />

    <v-data-table
      :headers="headers"
      :items="orders"
      hide-actions
      class="elevation-1"
      no-data-text="No orders available"
    >
      <template slot="items" slot-scope="props">
        <td>{{ props.item.OrderId }}</td>
        <td>{{ props.item.PaymentMethod }}</td>
        <td>{{ props.item.CreateDate | formatDate}}</td>
        <td>{{ props.item.UserSign.Name }}</td>

        <td class="justify-center">
          <v-btn color="deep-orange darken-1" class="white--text" @click="done(props.item)">Close</v-btn>
        </td>
      </template>
      <v-alert slot="no-results" :value="true" color="error" icon="warning">No data found for key:.</v-alert>
    </v-data-table>
  </div>
</template>



<script>
import Snackbar from "../../Helpers/Snackbar.vue";

export default {
  components: {
    Snackbar
  },
  data: () => ({
    url: "https://localhost:44389/api/Order/",
    snackbar: false,
    snackbarText: "",
    snackbarColor: "white",

    orders: [],
    file: null,
    headers: [
      {
        text: "Id",
        align: "left",
        value: "Id"
      },
      { text: "Payment method", value: "PaymentMethod" },
      { text: "Created date", value: "CreateDate" },
      { text: "Location", value: "UserSignId" },
      { text: "", value: "Actions", sortable: false }
    ]
  }),

  methods: {
    enableSnackbar(text, color) {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },
    fetchData() {
      this.orders = [];
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.axios
        .get(this.url + "Completed", config)
        .then(response => {
          this.orders = response.data;
        })
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {
          //console.log("zawsze");
        });
    },

    done(item) {
      const index = this.orders.indexOf(item);

      if (
        confirm(
          "Are you sure to close order nr: " + item.OrderId + " ?"
        )
      ) {
        var config = {
          headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
        };
        this.axios
          .post(this.url + "Close", item, config)
          .then(response => {
            this.orders.splice(index, 1);
            this.enableSnackbar(response.data, "success");
          })
          .catch(function(error) {
            console.log(error);
          });
      }
    }
  },
  created: function() {
    this.connection = new this.$signalR.HubConnectionBuilder()
      .withUrl("https://localhost:44389/orderHub")
      .configureLogging(this.$signalR.LogLevel.Error)
      .build();
  },
  mounted: function() {
    var thisContext = this;
    this.fetchData();
    //var connection = new signalR.HubConnection("https://localhost:44389/menuHub");
    this.connection.on("DoneOrder", function() {
      thisContext.fetchData();
      console.log("SignalRDoneORder");
    });

    this.connection.start().then(function() {
      console.log("SignalR started");
    });
  }
};
</script>