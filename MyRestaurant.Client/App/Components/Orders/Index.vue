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
      :search="search"
      hide-actions
      class="elevation-1"
      no-data-text="No orders available"
    >
      <template slot="items" slot-scope="props">
        <tr>
          <td>{{ props.item.OrderId }}</td>
          <td>{{ props.item.CreateDate | formatDate }}</td>
          <td>{{ props.item.UserSign.Name }}</td>

          <td class="justify-center">
            <v-btn color="success" @click="done(props.item)">Close</v-btn>
          </td>
        </tr>
        <tr v-for="line in props.item.OrderLines" style="background-color:#E8EAF6">
          <td>{{ line.Menu.Name }}</td>
          <td colspan="5">Quantity: {{ line.Quantity }}</td>
        </tr>
      </template>
      <v-alert
        slot="no-results"
        :value="true"
        color="error"
        icon="warning"
      >No data to key: {{search}}.</v-alert>
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
    search: "",
    passwordVisiblity: false,
    orders: [],
    file: null,
    headers: [
      {
        text: "Id",
        align: "left",
        value: "Id"
      },
      { text: "Created Date", value: "CreateDate" },
      { text: "Localization", value: "UserSignId" },
      { text: "", value: "Actions", sortable: false }
    ],
    dialog: false,
    editedIndex: -1,
    editedItem: {
      Id: 0,
      Login: "",
      Password: "",
      FristName: "",
      LastName: "",
      Avatar: ""
    },
    defaultItem: {
      Id: 0,
      Login: "",
      Password: "",
      FristName: "",
      LastName: "",
      Avatar: ""
    }
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
      console.log("headers", config);
      this.axios
        .get(this.url + "Running", config)
        .then(response => {
          console.log("data:", response.data);
          this.orders = response.data;
        })
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {
        });
    },

    done(item) {
      const index = this.orders.indexOf(item);

      console.log(item);
      if (
        confirm(
          "Are you sure to close order nr: " + item.OrderId + " ?"
        )
      ) {
        var config = {
          headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
        };

        this.axios
          .post(this.url + "Done", item, config)
          .then(response => {
            this.orders.splice(index, 1);
            this.enableSnackbar(response.data, "success");
          })
          .catch(function(error) {
            console.log(error);
          });
      }
    },

    close() {
      this.dialog = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },
    save() {
      //If  edit data
      if (this.editedIndex > -1) {
        this.axios
          .put(this.url, this.editedItem)

          .then(response => {
            Object.assign(this.users[this.editedIndex], this.editedItem);
            this.fetchData();
          })
          .catch(function(error) {
            console.log(error);
          })
          .then(function() {
          });
      }
      //If add new data
      else {
        console.log(this.editedItem);
        this.axios
          .post(this.url, this.editedItem)
          .then(response => {
            this.orders.push(this.editedItem);
            this.fetchData();
          })
          .catch(function(error) {
            console.log(error);
          })
          .then(function() {
          });
      }

      this.close();
    },

    changeAvatar() {
      if (confirm("Do you want to change avatar?")) {
        document.getElementById("fileInput").click();
      }
    },
    onFileChange(event, item) {
      if (confirm("Update image?")) {
        this.file = event.target.files[0];
        item.Avatar = this.file.name;
        this.saveFile();
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
    this.connection.on("NewOrder", function() {
      thisContext.fetchData();
      console.log("SignalRNewORder");
    });

    this.connection.start().then(function() {
      console.log("SignalR started");
    });
  },

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New item" : "Edit item";
    }
  }
};
</script>