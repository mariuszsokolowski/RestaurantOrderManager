 <template>
  <div>
    <snackbar
      v-if="snackbar"
      v-on:doneSnackbar="snackbar=false"
      :snackbarText="snackbarText"
      :snackbarColor="snackbarColor"
    />

    <v-dialog v-model="dialog" max-width="500px">
      <v-btn slot="activator" color="primary" dark class="mb-2">Add</v-btn>
      <v-card>
        <v-card-title class="headline grey lighten-2" primary-title>
          <span class="headline">{{ formTitle }}</span>
          <v-spacer></v-spacer>
          <!--<v-avatar
                @click="changeAvatar"
                size="6vw"
                 >
          <img :src="'https://localhost:44389/api/User/GetAvatar/'+editedItem.Avatar" alt="xx">
        </v-avatar>
        
              <input type="file"  :multiple="false" name="file" style="visibility:hidden"
               ref="fileInput" @change="onFileChange($event,editedItem)" id="fileInput" accept="image/*"
          >!-->
        </v-card-title>

        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm6 md4>
                  <v-text-field
                    v-model="editedItem.UserName"
                    label="Login"
                    :rules="userNameRules"
                    ref="editedItem.UserName"
                  ></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field
                    v-model="editedItem.Password"
                    :append-icon="passwordVisiblity ? 'visibility_off' : 'visibility'"
                    :rules="editedIndex<=0 || editedItem.Password.length>0 ? passwordRules : ''"
                    ref="editedItem.Password"
                    :type="passwordVisiblity ? 'text' : 'password'"
                    label="Password"
                    hint
                    counter
                    @click:append="passwordVisiblity = !passwordVisiblity"
                  ></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field
                    v-model="editedItem.FirstName"
                    label="First name"
                    :rules="firstNameRules"
                    ref="editedItem.FirstName"
                    required
                  ></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field
                    v-model="editedItem.LastName"
                    label="Last name"
                    :rules="lastNameRules"
                    ref="editedItem.LastName"
                  ></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 d-flex>
                  <v-select
                    v-if="editedIndex>-1"
                    :items="roles"
                    label="Choose role"
                    v-model="editedItem.RoleName"
                    outline
                    :rules="roleNameRules"
                    ref="editedItem.RoleName"
                  ></v-select>
                </v-flex>
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

    <v-data-table
      :headers="headers"
      :items="users"
      :search="search"
      hide-actions
      class="elevation-1"
    >
      <template slot="items" slot-scope="props">
        <!--<v-avatar
                size="4vw"
                 >
          <img :src="'https://localhost:44389/api/User/GetAvatar/'+props.item.Avatar" alt="">
        </v-avatar>!-->
        <td>{{ props.item.UserName }}</td>

        <td>
          <span v-for="_item in props.item.UserRoles">{{_item.Role.Name}}</span>
        </td>
        <td>{{ props.item.FirstName }}</td>

        <td>{{ props.item.LastName }}</td>

        <td class="justify-center">
          <v-icon medium color="primary" @click="editItem(props.item)">edit</v-icon>

          <v-icon
            color="error"
            medium
            v-show="props.item.IsBlocked===true"
            @click="activate(props.item)"
          >lock</v-icon>
          <v-icon
            color="success"
            medium
            v-show="props.item.IsBlocked===false"
            @click="deactivate(props.item)"
          >lock_open</v-icon>

          <v-icon medium color="red" @click="deleteItem(props.item)">delete</v-icon>
        </td>
      </template>
      <v-alert
        slot="no-results"
        :value="true"
        color="error"
        icon="warning"
      >No data found by key: {{search}}.</v-alert>
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
    snackbarColor: "white",
    url: "https://localhost:44389/api/User/",
    search: "",
    vueObject: this,
    passwordVisiblity: false,
    users: [],
    roles: [],
    file: null,
    headers: [
      {
        text: "Login",
        align: "left",
        value: "UserName"
      },

      { text: "Role", value: "Role" },
      { text: "First name", value: "FirstName" },
      { text: "Last name", value: "LastName" },
      { text: "", value: "Actions", sortable: false }
    ],
    dialog: false,
    editedIndex: -1,
    editedItem: {
      UserId: "",
      UserName: "",
      Password: "",
      RoleName: "Client",
      FirstName: "",
      LastName: ""
    },

    defaultItem: {
      UserName: "",
      Password: "",
      roleName: "Client"
    },
    //Rules
    valid: true,
    firstNameRules: [v => !!v || "Field first name is required."],
      lastNameRules: [v => !!v || "Field last name is required."],
    userNameRules: [
      v => !!v || "Field login is required.",
        v => v.length >= 4 || "Minimum number of characters is 4."
    ],
      roleNameRules: [v => !!v || "Granting permissions is required."],
    passwordRules: [
      v => !!v || "Field password is required.",
        v => v.length >= 6 || "Minimum number of characters is 6.",
        v => /[A-Z]+/.test(v) || "The password must have one uppercase letter.",
        v => /[^A-Za-z0-9]+/.test(v) || "The password must have one special character"
    ]
  }),
  created() {
    //this.initialize()
    this.fetchData();
    this.fetchRoles();
  },

  methods: {
    enableSnackbar(text, color) {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },
    fetchData() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.users = [];
      this.axios
        .get(this.url, config)
        .then(response => (this.users = response.data))
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
    },
    fetchRoles() {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      this.users = [];
      this.axios
        .get(this.url + "Roles", config)
        .then(response => (this.roles = response.data))
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
    },

    editItem(item) {
      this.editedIndex = this.users.indexOf(item);
      this.editedItem.UserName = Object.assign({}, item).UserName;
      this.editedItem.UserId = Object.assign({}, item).Id;
      this.editedItem.FirstName = Object.assign({}, item).FirstName;
      this.editedItem.LastName = Object.assign({}, item).LastName;
      console.log(Object.assign({}, item));
      if (Object.assign({}, item).UserRoles.length > 0) {
        this.editedItem.RoleName = Object.assign(
          {},
          item
        ).UserRoles[0].Role.Name;
      } else {
        this.editedItem.RoleName = "Client";
      }
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.users.indexOf(item);

      if (
        confirm('Are you sure to delete user: "' + item.UserName + '" ?')
      ) {
        this.axios
          .delete(this.url + "?id=" + item.Id)
          .then(response => this.users.splice(index, 1))
          .catch(function(error) {
            console.log(error);
          })
          .then(function() {
            //console.log("zawsze");
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
      if (!this.$refs.form.validate()) {
        return;
      }
      //If  edit data
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };
      if (this.editedIndex > -1) {
        this.axios
          .put(this.url, this.editedItem, config)

          .then(response => {
            //Object.assign(this.users[this.editedIndex], this.editedItem);
            this.fetchData();
            this.enableSnackbar("Data has been updated", "success");
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
        console.log(this.editedItem);
        this.axios
          .post(this.url, this.editedItem, config)
          .then(response => {
            this.users.push(this.editedItem);
            this.fetchData();
            this.enableSnackbar("User has benn added", "success");
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
    activate(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .put(this.url + "Activate", item, config)
        .then(response => {
          this.users[this.users.indexOf(item)].IsBlocked = false;
          this.enableSnackbar(response.data, "success");
        })
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
    },
    deactivate(item) {
      var config = {
        headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
      };

      this.axios
        .put(this.url + "Deactivate", item, config)
        .then(response => {
          this.users[this.users.indexOf(item)].IsBlocked = true;
          this.enableSnackbar(response.data, "success");
        })
        .catch(function(error) {
          console.error(error);
        })
        .then(function() {});
    }

    /* changeAvatar()
      {
        if(confirm("Czy chcesz zamienić avatar?"))
        {
          document.getElementById("fileInput").click();
        }
      },
      onFileChange(event,item){
 
              
            if(confirm("Zaktualizować zdjęcie?")){
              this.file = event.target.files[0]
              item.Avatar=this.file.name;
              this.saveFile()
            }
                            
            },

        saveFile(){


        var fileUrl = this.url+"UpdateFile"
       

        const formData = new FormData();
        formData.append('file',this.file, this.editedItem.Id +'.'+ this.file.name.split('.').pop())


           this.axios
            .post(fileUrl, formData )
            
            .then(response =>
            {
              this.fetchData()
              this.editedItem.Avatar=this.editedItem.Id +'.'+ this.file.name.split('.').pop()
              this.snackbarText='Zaktualizowano Avatar'
              this.snackbar=true
              this.snackbarColor="info"
            
      
           
            }
            
                        
            )
            .catch(function (error) {
                alert('Blad podczas aktualizacji')
            })
            .then(function () {
          
            }); 

   
      }*/
  },

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New item" : "Edit item";
    }
  }
};
</script>