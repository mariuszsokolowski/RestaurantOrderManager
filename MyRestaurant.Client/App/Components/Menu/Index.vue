<template>
  <div>
    <snackbar
      v-if="snackbar"
      v-on:doneSnackbar="snackbar=false"
      :snackbarText="snackbarText"
      :snackbarColor="snackbarColor"
    />
    <v-toolbar flat color="white">
      <v-toolbar-title>Menu</v-toolbar-title>

      <v-divider class="mx-2" inset vertical></v-divider>

      <v-spacer></v-spacer>

      <v-dialog v-model="dialog" max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">Dodaj</v-btn>
        <v-card>
          <v-card-title class="headline grey lighten-2" primary-title>
            <span class="headline">{{ formTitle }}</span>
          </v-card-title>

          <v-card-text>
            <v-form ref="form" v-model="valid" lazy-validation>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs12 sm6 md4>
                    <v-text-field
                      v-model="editedItem.Name"
                      label="Name"
                      :rules="nameRules"
                      ref="editedItem.Name"
                      required
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md4>
                    <v-text-field
                      v-model="editedItem.Description"
                      label="Description"
                      :rules="descriptionRules"
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md4>
                    <v-text-field v-model="editedItem.Price" label="Cena [PLN]" :rules="priceRules"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md4 v-show="this.editedIndex!==-1">
                    <v-text-field
                      prepend-icon="attach_file"
                      v-model="editedItem.Image"
                      label="Image"
                      @click.native="onFocus"
                      readonly="readonly"
                    ></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md4>
                    <v-checkbox v-model="editedItem.Promotion" label="Promotion"></v-checkbox>
                  </v-flex>

                  <input
                    type="file"
                    :multiple="false"
                    name="file"
                    style="visibility:hidden"
                    ref="fileInput"
                    @change="onFileChange($event,editedItem)"
                    id="fileInput"
                    accept="image/*"
                  >
                </v-layout>
              </v-container>
            </v-form>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" flat @click.native="close">Close</v-btn>
            <v-btn color="blue darken-1" flat @click.native="save">Save</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-toolbar>

    <v-flex xs12 sm6 md4 style="margin-left:auto">
      <v-text-field v-model="search" append-icon="search" label="Search" single-line hide-details></v-text-field>
    </v-flex>
    <v-data-table
      :headers="headers"
      :items="menu"
      :search="search"
      hide-actions
      class="elevation-1"
    >
      <template slot="items" slot-scope="props">
        <td>
          <v-avatar size="6vw">
            <img :src="'https://localhost:44389/api/Menu/GetImg/'+props.item.Image" alt>
          </v-avatar>
          {{ props.item.Name }}
        </td>
        <td>{{ props.item.Description }}</td>
        <td>{{ props.item.Price }}</td>

        <td>
          <v-checkbox v-model="props.item.Promotion" color="red" hide-details readonly></v-checkbox>
        </td>
        <td class="justify-center">
          <v-icon medium color="primary" @click="editItem(props.item)">edit</v-icon>
          <!-- <v-icon medium color="red" @click="deleteItem(props.item)">delete</v-icon>-->
          <v-icon
            color="error"
            medium
            v-show="props.item.Inactive===true"
            @click="activateMenu(props.item)"
          >lock</v-icon>
          <v-icon
            color="success"
            medium
            v-show="props.item.Inactive===false"
            @click="deactivateMenu(props.item)"
          >lock_open</v-icon>
        </td>
      </template>
      <v-alert
        slot="no-results"
        :value="true"
        color="error"
        icon="warning"
      >No data found to key: {{search}}.</v-alert>
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
    snackbar: false,
    snackbarText: "",
    snackbarColor: "",

    dataa: null,
    file: null,
    search: "",
    url: "https://localhost:44389/api/Menu/",
    filename: "",
    dialog: false,
    headers: [
      {
        text: "Name",
        align: "left",
        value: "Name"
      },
      { text: "Description", value: "Description" },
      { text: "Price", value: "Price" },
      { text: "Image", value: "Image" },
      { text: "", value: "Name", sortable: false }
    ],
    menu: [],
    editedIndex: -1,
    editedItem: {
      Name: "",
      Description: "",
      Price: 0,
      Image: "",
      Promotion: false,
      file: null
    },
    defaultItem: {
      Name: "",
      Descritpion: "",
      Price: 0,
      Image: "",
      Promotion: false
    },
    ///Rules
    valid: true,
    nameRules: [v => !!v || "Name is requied."],
      descriptionRules: [v => !!v || "Description is requied."],
      priceRules: [v => v > 0 || "Price is requied."]
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New item" : "Edit item";
    },
    nameErrors() {
      const errors = [];
      if (!this.$v.editedItem.Name.$dirty) return errors;
      !this.$v.editedItem.Name.maxLength &&
        errors.push("Name must be at most 10 characters long");
      !this.$v.name.required && errors.push("Name is required.");
      return errors;
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    //this.initialize()
    this.fetchData();
  },

  methods: {
    enableSnackbar(text, color) {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },
    fetchData() {
      this.menu = [];
      this.axios
        .get(this.url)
        .then(response => (this.menu = response.data))
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {
        });
    },
    activateMenu(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .post(this.url + "Activate", item, config)
        .then(response => {
          this.menu[this.menu.indexOf(item)].Inactive = false;
          this.enableSnackbar(response.data, "success");
        })
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
    },
    deactivateMenu(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .post(this.url + "Deactivate", item, config)
        .then(response => {
          this.menu[this.menu.indexOf(item)].Inactive = true;
          this.enableSnackbar(response.data, "success");
        })
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
    },

    editItem(item) {
      this.editedIndex = this.menu.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      const index = this.menu.indexOf(item);

        if (confirm('Are you sure you want to delete this item: "' + item.Name + '" ?')) {
        this.axios
          .delete(this.url + "?id=" + item.MenuId, config)
          .then(response => this.menu.splice(index, 1))
          .catch(error => {
            this.enableSnackbar(error.response.data, "error");
          })
          .then(function() {
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
    onFileChange(event, item) {
      if (confirm("Update image?")) {
        this.file = event.target.files[0];

        item.Image = this.file.name;
        this.saveFile();
      }
    },
    save() {
      if (!this.$refs.form.validate()) {
        return;
      }
      const test = {
        model: this.editedItem
      };

      //If  edit data
      if (this.editedIndex > -1) {
        this.axios
          .put(this.url, this.editedItem)

          .then(response => {
            this.fetchData();
            this.enableSnackbar(response.data, "success");
          })
          .catch(function(error) {
            console.log(error);
          })
          .then(function() {
            //console.log("zawsze");
          });
      }
      //If add new data
      else {
        this.axios
          .post(this.url, this.editedItem)
          .then(response => {
            this.menu.push(this.editedItem);
            this.fetchData();
          })
          .catch(function(error) {
            console.log(error);
          })
          .then(function() {
            //console.log("zawsze");
          });
      }

      this.close();
    },
    saveFile() {
      var fileUrl = this.url + "UpdateFile";
      const newFileName =
        this.editedItem.MenuId + "." + this.file.name.split(".").pop();
      console.log("new file name", newFileName);

      const formData = new FormData();
      formData.append("file", this.file, newFileName);

      this.axios
        .post(fileUrl, formData)

        .then(response => {
          this.fetchData();
          this.editedItem.Image =
            this.editedItem.ID + "." + this.file.name.split(".").pop();
          this.snackbarText = "Image has benn updated";
          this.snackbar = true;
          this.snackbarColor = "info";
        })
        .catch(function(error) {
          alert("Error during update");
        })
        .then(function() {});
    },
    onFocus() {
      if (!this.disabled) {
        debugger;
        this.$refs.fileInput.click();
      }
    }
  }
};
</script>