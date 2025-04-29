async function initMap() {
    await ymaps3.ready;

    const { YMap, YMapDefaultSchemeLayer, YMapListener } = ymaps3;


    const map = new YMap(
        document.getElementById('map'),
        {
            location: {
                center: [49.668023, 58.603595],
                zoom: 10
            }
        }
    );

    const mapListener = new YMapListener({
        layer: 'any',
    })

    const behaviorEndHandler = ({ type }) => {
        if (type === 'drag') {
            const center = map.center;
            const address = geocode(center);
            console.log('Текущий адрес:', address);
        }
    };

    mapListener.update({ onActionEnd: behaviorEndHandler });

    map.addChild(mapListener);
    map.addChild(new YMapDefaultSchemeLayer());

    // Функция для геокодирования координат в адрес
    async function geocode(coords) {
        const response = await fetch(`https://geocode-maps.yandex.ru/1.x/?format=json&geocode=${coords[0]},${coords[1]}&apikey=46514e07-4df3-4059-9c68-309be409aa4b`);
        const data = await response.json();
        const address = data.response.GeoObjectCollection.featureMember[0].GeoObject.metaDataProperty.GeocoderMetaData.Address.Components;
        var addressArray = address.filter(x => x.kind == 'locality' || x.kind == 'street' || x.kind == 'house');
        var city = document.querySelector('#city');
        var street = document.querySelector('#street');
        var house = document.querySelector('#house');
        city.value = addressArray[0].name;
        street.value = addressArray[1].name;
        house.value = addressArray[2].name;
        return address;
    }
}

initMap();