$(document).ready(async function (){
    const ctx = document.getElementById('salaryChart').getContext('2d');
    let users = await getUsers();
    let backgroundColors = users.map(obj => randomRGB());
    const salaryChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: users.map(obj => obj["firstName"] + " " + obj["lastName"]),
            datasets: [{
                label: 'Imie i nazwisko',
                data: users.map(obj => obj['salary']),
                backgroundColor: backgroundColors,
                borderColor: '#00bcd4',
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: {
                    position: 'bottom'
                },
                title: {
                    display: true,
                    text: 'Wynagrodzenia'
                }
            }
        }
    })
    
    const salariesDiv = $('.salaries');
    
    users.forEach(obj => {
        let user = obj;
        let salary = user['salary'];
        let firstName = user['firstName'];
        let lastName = user['lastName'];
        let id = user['id'];
        let html = `<div class="salary" id="${id}">
                        <div class="salary-name h3">${firstName} ${lastName}</div>
                        <div class="salary-value">${salary} zł</div>
                    </div>`;
        salariesDiv.append(html);
    });
    
    
    let sumSalaries = 0;
    users.forEach(obj => {
        sumSalaries += obj['salary'];
    });
    
    salariesDiv.append(`<div class="salary text-center">
                            <div class="salary-name h3">Lacznie</div>
                            <div class="salary-value">${sumSalaries} zł</div>
                        </div>`);
    
    
    let offers = await getOffers();
    
    offers.forEach(obj => {
        let offer = obj;
        let id = offer['id'];
        let html = `<div class="offer" id="${id}">
                        <div class="offer-name h3">${offer['name']}</div>
                    </div>`;
        $('.offers').append(html);
    });
    
    
    let applications = await getApplications();
    
    applications.forEach(obj => {
        let application = obj.application;
        let offer = obj.offer;
        let user = obj.user;
        let id = application['id'];
        let html = `<div class="application" id="${id}">
                        <div class="application-name h3">${user['firstName']} ${user['lastName']} - ${offer["role"]}</div>
                    </div>`;
        $('.applications').append(html);
    });
    
    
})




