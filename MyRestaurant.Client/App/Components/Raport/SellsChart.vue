<script>
import { Bar } from "vue-chartjs";

export default {
  data: function() {
    return {
      options: {
        maintainAspectRatio: false,
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true
              }
            }
          ]
        }
      }
    };
  },
  extends: Bar,
  props: ["dataChart"],

  methods: {
    fetchData() {
      return this.dataChart;
    }
  },
  mounted() {
    // Overwriting base render method with actual data.

    this.renderChart(
      {
        labels: this.dataChart.map(x => x.Name),
        datasets: [
          {
            label: "Month",
            backgroundColor: "teal",
            data: this.dataChart.map(x => x.Total)
          }
        ]
      },
      this.options
    );
  }
};
</script>