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
      :items="notifiactions"
      hide-actions
      class="elevation-1"
      no-data-text="No active notification"
    >
      <template slot="items" slot-scope="props">
        <td>{{ props.item.NotificationID }}</td>
        <td>{{ props.item.UserSign.Name }}</td>
        <td>{{ props.item.CreateDate | formatDate}}</td>

        <td class="justify-center">
          <v-btn
            color="deep-orange darken-1"
            class="white--text"
            @click="done(props.item)"
            v-if="props.item.UserAssigned !== null"
          >Close</v-btn>

          <v-btn
            color="deep-orange darken-1"
            class="white--text"
            @click="assign(props.item)"
            v-show="props.item.UserAssigned === null"
          >Assign</v-btn>
        </td>
      </template>
      <v-alert slot="no-results" :value="true" color="error" icon="warning">Brak danych dla klucza:.</v-alert>
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
    url: "https://localhost:44389/api/Notification/",
    snackbar: false,
    snackbarText: "",
    snackbarColor: "",
    notifiactions: [],
    headers: [
      {
        text: "Id",
        align: "left",
        value: "NotificationID"
      },
      { text: "Location", value: "UserSign" },
      { text: "Created Date", value: "CreateDate" },
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
      // this.notifiactions = [];
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.axios
        .get(this.url, config)
        .then(response => {
          this.notifiactions = response.data;

          //var test=response.data
        })
        .catch(error => {})
        .then(function() {});
    },

    done(item) {
      const index = this.notifiactions.indexOf(item);

      if (
        confirm(
          "Are you sure to close notification nr: " +
            item.NotificationID +
            " ?"
        )
      ) {
        var config = {
          headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
        };
        this.axios
          .post(this.url + "Done", item, config)
          .then(response => {
            this.enableSnackbar(response.data, "success");
            this.notifiactions.splice(index, 1);
          })
          .catch(function(error) {
            console.error(error);
          });
      }
    },

    assign(item) {
      const index = this.notifiactions.indexOf(item);

      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.axios
        .post(this.url + "Assign", item, config)
        .then(response => {
          this.fetchData();
          this.enableSnackbar(response.data, "success");
        })
        .catch(function(error) {
          console.log(error);
        });
    }
  },
  created: function() {
    this.connection = new this.$signalR.HubConnectionBuilder()
      .withUrl("https://localhost:44389/notificationHub")
      .configureLogging(this.$signalR.LogLevel.Error)
      .build();
  },
  mounted: function() {
    var thisContext = this;
    this.fetchData();
    //var connection = new signalR.HubConnection("https://localhost:44389/menuHub");
    this.connection.on("NewNotification", function() {
      thisContext.fetchData();
      console.log("SignalRNotificationHub");
    });

    this.connection.start().then(function() {
      console.log("SignalR started");
    });
  }
};
</script>