<template>
  <v-app>
    <Nav></Nav>
    <div class="container">
      <router-view></router-view>
    </div>
  </v-app>
</template>


<style>
a {
  text-decoration: none;
  color: black !important;
}

input[type="number"] {
  text-align: center;
}
</style>

<script>
import Nav from "./Components/Nav.vue";

export default {
  components: {
    Nav
  }
};
</script>