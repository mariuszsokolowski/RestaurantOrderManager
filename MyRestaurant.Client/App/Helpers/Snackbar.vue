<template>
  <v-snackbar
    v-model="snackbar"
    :bottom="y === 'bottom'"
    :left="x === 'left'"
    :multi-line="mode === 'multi-line'"
    :right="x === 'right'"
    :timeout="snackbarTimeout"
    :top="y === 'top'"
    :vertical="mode === 'vertical'"
    :color="snackbarColor"
  >
    {{ snackbarText }}
    <v-btn color="black" flat @click="snackbar = false">X</v-btn>
  </v-snackbar>
</template>

<script>
export default {
  props: ["snackbarText", "snackbarColor"],
  data() {
    return {
      snackbar: true,
      snackbarTimeout: 6000,

      y: "top",
      x: null,
      mode: ""
    };
  },
  watch: {
    snackbar: function(val) {
      if (!val) {
        this.$emit("doneSnackbar");
      }
    }
  }
};
</script>

<style>
</style>
