function exportPdf(fileName, baliseId, url) {
    console.log('Chemin d\'origine: ' + window.location.origin);
    var originPath = window.location.origin;
    console.log(`${originPath}/lib/bootstrap/dist/css/bootstrap.css`)
    var contentHtml = `
        <html>
            <head>
                <link rel="stylesheet" href="${originPath}/lib/bootstrap/dist/css/bootstrap.css">
            </head>
            <body>
              <div class="container-scroller">                                               
                    <div class="container-fluid page-body-wrapper">
                        <div class="main-panel">
                            <div class="content-wrapper">
                                <div class="row">
                                    ${document.getElementById(baliseId).innerHTML}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </body>
        </html>
    `;

    //console.log(contentHtml);
    console.log('Export to pdf ' + fileName);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", url, true);
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    xhr.responseType = 'blob';

    xhr.onload = function () {
        if (xhr.status === 200) {
            var blob = xhr.response;
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName + ".pdf";
            link.click();
        } else {
            console.error('Error: ' + xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Request failed');
    }

    var requestData = {
        htmlContent: contentHtml,
        fileName: fileName
    };

    xhr.send(JSON.stringify(requestData));
}

function exportExcel(fileName, baliseId, url) {
    console.log('Chemin d\'origine: ' + window.location.origin);

    var contentHtml = document.getElementById(baliseId).innerHTML;

    //console.log(contentHtml);
    console.log('Export to excel ' + fileName);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", url, true);
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    xhr.responseType = 'blob';

    xhr.onload = function () {
        if (xhr.status === 200) {
            var blob = xhr.response;
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName + ".xls";
            link.click();
        } else {
            console.error('Error: ' + xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Request failed');
    }

    var requestData = {
        htmlContent: contentHtml,
        fileName: fileName
    };

    xhr.send(JSON.stringify(requestData));
}