<template>
  <div>
    <v-bottom-nav :active.sync="bottomNav" :color="color" :value="true" shift>
      <v-btn dark @click="fetchSaleDate">
        <span>Sprzedaż</span>
        <v-icon>attach_money</v-icon>
      </v-btn>

      <v-btn dark @click="fetchYearSaleDate">
        <span>Menu</span>
        <v-icon>bar_chart</v-icon>
      </v-btn>

      <v-btn dark @click="fetchEmployeeData">
        <span>Pracownicy</span>
        <v-icon>people</v-icon>
      </v-btn>
    </v-bottom-nav>
    <div class="text-xs-center">
      <v-progress-circular
        :size="70"
        :width="7"
        color="purple"
        indeterminate
        justify-center
        v-if="loadingData"
      ></v-progress-circular>
    </div>

    <v-data-table
      :headers="saleHeaders"
      :items="saleData"
      class="elevation-1"
      v-if="saleData.length>0"
      rows-per-page-text="Wierszy na stronie"
    >
      <template slot="items" slot-scope="props">
        <td>{{ props.item.MenuName }}</td>
        <td>
          <v-rating
            v-model="props.item.Rating"
            color="yellow darken-3"
            background-color="grey darken-1"
            empty-icon="$vuetify.icons.ratingFull"
            half-increments
            hover
            readonly
          ></v-rating>
        </td>
        <td>{{ props.item.OrderCount }}</td>
        <td>{{ props.item.OrderQuantity }}</td>
        <td>{{ Math.round(((props.item.OrderQuantity/TotalOrders)*100)) }}%</td>
        <td>{{ props.item.Cash }} zł</td>
      </template>
    </v-data-table>

    <v-data-table
      :headers="employeHeader"
      :items="employeeData"
      class="elevation-1"
      v-if="employeeData.length>0"
      rows-per-page-text="Wierszy na stronie"
    >
      <template slot="items" slot-scope="props">
        <td>{{ props.item.FullName }}</td>
        <td>
          <v-rating
            v-if="props.item.WaiterAssignCalls>0"
            v-model="props.item.Rating"
            color="yellow darken-3"
            background-color="grey darken-1"
            empty-icon="$vuetify.icons.ratingFull"
            half-increments
            hover
            readonly
          ></v-rating>
          <span v-if="props.item.WaiterAssignCalls<=0" style="color:red">Brak oceny</span>
        </td>
        <td>{{ props.item.TotalCalls }}</td>
        <td>{{ props.item.WaiterAssignCalls }}</td>
      </template>
    </v-data-table>

    <SellsChart v-if="dataChart.length>0" :dataChart="dataChart"/>
  </div>
</template>

<script>
import SellsChart from "./SellsChart.vue";

export default {
  components: { SellsChart },
  data: function() {
    return {
      url: "https://localhost:44389/api/Report/",
      authConfig: null,
      loadingData: false,
      dataChart: [],
      datacollection: null,
      TotalOrders: 0,
      saleHeaders: [
        {
          text: "Menu",
          align: "left",
          value: "MenuName"
        },
        { text: "Średnia ocena", value: "Rating" },
        { text: "Liczba zamówień", value: "OrderCount" },
        { text: "Ilość na zamówieniu", value: "OrderQuantity" },
        { text: "% zamówień", value: "TotalProcent" },
        { text: "Obrót", value: "Cash" }
      ],
      employeHeader: [
        {
          text: "Kelner",
          align: "left",
          value: "FullName"
        },
        { text: "Średnia ocena", value: "Rating" },
        { text: "Liczba wszystkich przywołań", value: "TotalCalls" },
        { text: "Liczba przypisanych przywołań", value: "WaiterAssignCalls" }
      ],
      bottomNav: 4,
      saleData: [],
      employeeData: []
    };
  },

  methods: {
    fetchSaleDate() {
      this.clearAllData();
      this.loadingData = true;
      var context = this;
      this.axios
        .get(this.url + "Sale", this.authConfig)
        .then(function(response) {
          for (var index = 0; index < response.data.length; index++) {
            var element = response.data[index];

            context.TotalOrders += element.OrderQuantity;
          }
          context.loadingData = false;
          context.saleData = response.data;
        })
        .catch(function(error) {
          context.loadingData = false;
          console.error(error);
        });
    },
    fetchYearSaleDate() {
      this.clearAllData();
      this.loadingData = true;
      var context = this;
      this.axios
        .get(this.url + "YearSale", this.authConfig)
        .then(function(response) {
          context.loadingData = false;
          context.dataChart = response.data;
          context.dataChart = context.renameMonth(context.dataChart);
        })
        .catch(function(error) {
          context.loadingData = false;
          console.error(error);
        });
    },
    fetchEmployeeData() {
      this.clearAllData();
      var context = this;
      this.loadingData = true;
      this.axios
        .get(this.url + "Employee", this.authConfig)
        .then(function(response) {
          context.loadingData = false;
          console.log(response.data);
          context.employeeData = response.data;
        })
        .catch(function(error) {
          console.error(error);
          context.loadingData = false;
        });
    },

    clearAllData() {
      this.employeeData = [];
      this.saleData = [];
      this.dataChart = [];
    },
    renameMonth(item) {
      for (var i = 0; i < item.length; i++) {
        switch (item[i].Name) {
          case 1:
            item[i].Name = "Styczeń";
            break;
          case 2:
            item[i].Name = "Luty";
            break;
          case 3:
            item[i].Name = "Marzec";
            break;
          case 4:
            item[i].Name = "Kwieceiń";
            break;
          case 5:
            item[i].Name = "Maj";
            break;
          case 6:
            item[i].Name = "Czerwiec";
            break;
          case 7:
            item[i].Name = "Lipiec";
            break;
          case 8:
            item[i].Name = "Sierpien";
            break;
          case 9:
            item[i].Name = "Wrzesień";
            break;
          case 10:
            item[i].Name = "Październik";
            break;
          case 11:
            item[i].Name = "Listopad";
            break;
          case 12:
            item[i].Name = "Grudzień";
            break;
        }
      }
      return item;
    }
  },
  computed: {
    color() {
      switch (this.bottomNav) {
        case 0:
          return "blue-grey";
        case 1:
          return "teal";
        case 2:
          return "brown";
        case 3:
          return "red";
        case 4:
          return "indigo";
      }
    }
  },

  mounted() {
    this.authConfig = {
      headers: { Authorization: "bearer " + this.$cookies.get("authToken") }
    };
  }
};
</script>
