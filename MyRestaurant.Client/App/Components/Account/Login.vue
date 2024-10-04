<template>
  <v-content>
    <snackbar
      v-if="snackbar"
      v-on:doneSnackbar="snackbar=false"
      :snackbarText="snackbarText"
      :snackbarColor="snackbarColor"
    />
    <v-container fluid fill-height>
      <v-layout align-center justify-center>
        <v-flex xs12 sm8 md4>
          <v-card class="elevation-12">
            <v-toolbar dark color="primary">
              <v-toolbar-title>Logowanie do systemu</v-toolbar-title>
              <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
              <v-form ref="form">
                <v-text-field
                  prepend-icon="person"
                  name="login"
                  label="Login"
                  type="text"
                  v-model="LoginModel.Login"
                  :rules="userNameRules"
                ></v-text-field>
                <v-text-field
                  prepend-icon="lock"
                  name="password"
                  label="Hasło"
                  id="password"
                  type="password"
                  v-model="LoginModel.Password"
                  :rules="passwordRules"
                ></v-text-field>
              </v-form>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="primary" @click="login">Zaloguj</v-btn>
            </v-card-actions>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
  </v-content>
</template>


    <script>
import { eventBus } from "../../index";
import Snackbar from "../../Helpers/Snackbar.vue";

export default {
  components: {
    Snackbar
  },
  data: () => ({
    url: "https://localhost:44389/api/Account/",
    snackbar: false,
    snackbarText: "",
    snackbarColor: "white",

    LoginModel: {
      Login: "",
      Password: ""
    },
    //Rules
    valid: true,
    userNameRules: [
      v => !!v || "Pole login jest wymagane.",
      v => v.length >= 4 || "Minimalna ilość znaków to 4."
    ],

    passwordRules: [
      v => !!v || "Pole hasło jest wymagane.",
      v => v.length >= 6 || "Minimalna ilość znaków to 6.",
      v => /[A-Z]+/.test(v) || "Hasło musi mieć jedną duża literę.",
      v => /[^A-Za-z0-9]+/.test(v) || "Hasło musi posiadać znak specjalny"
    ]
  }),

  methods: {
    enableSnackbar(text, color) {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },
    login() {
      if (!this.$refs.form.validate()) {
        return;
      }

      this.axios
        .post(this.url + "Login", this.LoginModel, null)

        .then(response => {
          this.$cookies.set("authToken", response.data.Token);
          this.$cookies.set("authRole", response.data.Role);
          eventBus.changeCookiesAuth();
          if (response.data.Role == "Waiter") {
            this.$router.push("/Orders/Completed");
          } else if (response.data.Role == "Cook") {
            this.$router.push("/Orders/");
          } else {
            this.$router.push("/");
          }
        })
        .catch(error => {
          this.enableSnackbar(error.response.data, "error");
        });
    }
  }
};
</script>