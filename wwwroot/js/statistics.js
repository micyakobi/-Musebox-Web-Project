
    Highcharts.chart('container', {
        data: {
        table: 'datatable'
        },
        chart: {
        type: 'column',
            backgroundColor: 'transparent'
        },
        title: {
        text: 'Type Statistics'
        },
        yAxis: {
        allowDecimals: false,
            title: {
        text: 'Number of Items'
            }
        },
        tooltip: {
        formatter: function () {
                return '<b>' + this.series.name + '</b><br />' +
                    this.point.y + ' ' + this.point.name.toLowerCase();
            }
        }

    });

        Highcharts.chart('container2', {
            data: {
            table: 'datatable2'
        },
        chart: {
            type: 'pie',
            backgroundColor: 'transparent'
        },
        title: {
            text: 'Sales Statistics'
        },
        subtitle: {
            text: 'By Item Name'
        },
        plotOptions: {
            series: {
            dataLabels: {
            enabled: true,
                    format: '{point.name}: {point.percentage:.2f} %'
                }
            }

        },

        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.percentage:.2f}%</b> of total<br />'
        }
    });
