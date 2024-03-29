﻿async function postData(url = '', data = {}){
    // Default options are marked with *
    const response = await fetch(url, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json'
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        redirect: 'follow', // manual, *follow, error
        referrer: 'no-referrer', // no-referrer, *client
        body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    return await response.json(); // parses JSON response into native JavaScript objects
}

async function getUsers(){
    return await postData('Dashboard/getUsers', {});
}

async function getOffers(){
    return await postData('Dashboard/GetOffers', {});
}

async function getApplications() {
    return await postData('Dashboard/GetApplications', {});
}

async function getEvents() {
    return await postData('Meeting/GetEvents', {});
}

async function getProjects() {
    return await postData('Project/GetProjects', {});
}

const randomNum = () => Math.floor(Math.random() * (235 - 52 + 1) + 52);

const randomRGB = () => `rgb(${randomNum()}, ${randomNum()}, ${randomNum()})`;

function colorHash(inputString){
    let sum = 0;

    for(let i in inputString){
        sum += inputString.charCodeAt(i);
    }

    let r = ~~(('0.' + Math.sin(sum + 1).toString().substr(6)) * 256);
    let g = ~~(('0.' + Math.sin(sum + 2).toString().substr(6)) * 256);
    let b = ~~(('0.' + Math.sin(sum + 3).toString().substr(6)) * 256);

    let rgb = "rgb(" + r + ", " + g + ", " + b + ")";

    let hex = "#";

    hex += ("00" + r.toString(16)).substr(-2,2).toUpperCase();
    hex += ("00" + g.toString(16)).substr(-2,2).toUpperCase();
    hex += ("00" + b.toString(16)).substr(-2,2).toUpperCase();

    return {
        r: r
        ,g: g
        ,b: b
        ,rgb: rgb
        ,hex: hex
    };
}
