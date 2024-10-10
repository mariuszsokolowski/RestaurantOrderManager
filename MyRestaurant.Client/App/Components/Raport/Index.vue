<template>
    <div>
        <v-bottom-nav :active.sync="bottomNav" :color="color" :value="true" shift>
            <v-btn dark @click="fetchSaleDate">
                <span>Sale</span>
                <v-icon>attach_money</v-icon>
            </v-btn>

            <v-btn dark @click="fetchYearSaleDate">
                <span>Menu</span>
                <v-icon>bar_chart</v-icon>
            </v-btn>

            <v-btn dark @click="fetchEmployeeData">
                <span>Employees</span>
                <v-icon>people</v-icon>
            </v-btn>
        </v-bottom-nav>
        <div class="text-xs-center">
            <v-progress-circular :size="70"
                                 :width="7"
                                 color="purple"
                                 indeterminate
                                 justify-center
                                 v-if="loadingData"></v-progress-circular>
        </div>

        <v-data-table :headers="saleHeaders"
                      :items="saleData"
                      class="elevation-1"
                      v-if="saleData.length>0"
                      rows-per-page-text="Rows on page">
            <template slot="items" slot-scope="props">
                <td>{{ props.item.MenuName }}</td>
                <td>
                    <v-rating v-model="props.item.Rating"
                              color="yellow darken-3"
                              background-color="grey darken-1"
                              empty-icon="$vuetify.icons.ratingFull"
                              half-increments
                              hover
                              readonly></v-rating>
                </td>
                <td>{{ props.item.OrderCount }}</td>
                <td>{{ props.item.OrderQuantity }}</td>
                <td>{{ Math.round(((props.item.OrderQuantity/TotalOrders)*100)) }}%</td>
                <td>{{ props.item.Cash }} zł</td>
            </template>
        </v-data-table>

        <v-data-table :headers="employeHeader"
                      :items="employeeData"
                      class="elevation-1"
                      v-if="employeeData.length>0"
                      rows-per-page-text="Rows on page">
            <template slot="items" slot-scope="props">
                <td>{{ props.item.FullName }}</td>
                <td>
                    <v-rating v-if="props.item.WaiterAssignCalls>0"
                              v-model="props.item.Rating"
                              color="yellow darken-3"
                              background-color="grey darken-1"
                              empty-icon="$vuetify.icons.ratingFull"
                              half-increments
                              hover
                              readonly></v-rating>
                    <span v-if="props.item.WaiterAssignCalls<=0" style="color:red">No rate</span>
                </td>
                <td>{{ props.item.TotalCalls }}</td>
                <td>{{ props.item.WaiterAssignCalls }}</td>
            </template>
        </v-data-table>

        <SellsChart v-if="dataChart.length>0" :dataChart="dataChart" />
    </div>
</template>

<script>
    import SellsChart from "./SellsChart.vue";

    export default {
        components: { SellsChart },
        data: function () {
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
                    { text: "Avg rate", value: "Rating" },
                    { text: "Orders count", value: "OrderCount" },
                    { text: "Quantity on order", value: "OrderQuantity" },
                    { text: "% orders", value: "TotalProcent" },
                    { text: "Rotation", value: "Cash" }
                ],
                employeHeader: [
                    {
                        text: "Waiter",
                        align: "left",
                        value: "FullName"
                    },
                    { text: "Avg rate", value: "Rating" },
                    { text: "Totals calls", value: "TotalCalls" },
                    { text: "Assign calls", value: "WaiterAssignCalls" }
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
                    .then(function (response) {
                        for (var index = 0; index < response.data.length; index++) {
                            var element = response.data[index];

                            context.TotalOrders += element.OrderQuantity;
                        }
                        context.loadingData = false;
                        context.saleData = response.data;
                    })
                    .catch(function (error) {
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
                    .then(function (response) {
                        context.loadingData = false;
                        context.dataChart = response.data;
                        context.dataChart = context.renameMonth(context.dataChart);
                    })
                    .catch(function (error) {
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
                    .then(function (response) {
                        context.loadingData = false;
                        console.log(response.data);
                        context.employeeData = response.data;
                    })
                    .catch(function (error) {
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
                            item[i].Name = "January";
                            break;
                        case 2:
                            item[i].Name = "February";
                            break;
                        case 3:
                            item[i].Name = "March";
                            break;
                        case 4:
                            item[i].Name = "April";
                            break;
                        case 5:
                            item[i].Name = "May";
                            break;
                        case 6:
                            item[i].Name = "June";
                            break;
                        case 7:
                            item[i].Name = "July";
                            break;
                        case 8:
                            item[i].Name = "August";
                            break;
                        case 9:
                            item[i].Name = "Wrzesień";
                            break;
                        case 10:
                            item[i].Name = "September";
                            break;
                        case 11:
                            item[i].Name = "November";
                            break;
                        case 12:
                            item[i].Name = "December";
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
