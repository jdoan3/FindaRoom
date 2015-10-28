var geocoder;
var map = null;
var marker = [];
var autocomplete = [];
var saved_widget = null;
var address = null;
var infowindow= null;
function initialize() {
    geocoder = new google.maps.Geocoder();
    var latlng = new google.maps.LatLng(62.021528, 15.36438);
    var mapOptions = {
        zoom: 5,
        center: latlng,
        panControl: true,
        panControlOptions: {
        position: google.maps.ControlPosition.RIGHT_TOP
    },
        zoomControl: true,
        zoomControlOptions: {
        style: google.maps.ZoomControlStyle.LARGE,
        position: google.maps.ControlPosition.RIGHT_TOP
    }
    }
    infowindow = new google.maps.InfoWindow();
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

    var autocomplete = new google.maps.places.Autocomplete(document.getElementById('pac-input'));
    autocomplete.bindTo('bounds', map);

    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            return;
        }
        map.setCenter(place.geometry.location);
        map.setZoom(12);


        var address = '';
        if (place.address_components) {
            address = [
              (place.address_components[0] && place.address_components[0].short_name || ''),
              (place.address_components[1] && place.address_components[1].short_name || ''),
              (place.address_components[2] && place.address_components[2].short_name || '')
            ].join(' ');
        }
        var distanceWidget = new DistanceWidget(place, map, place.geometry.location, place.name, address, infowindow);
        saved_widget = distanceWidget;
        var info = document.getElementById('info');
        info.innerHTML = 'Position: ' + saved_widget.get('position') + ', distance: ' +
  saved_widget.get('distance');
        google.maps.event.addListener(distanceWidget, 'distance_changed', function () {
            displayInfo(distanceWidget);
        });

        google.maps.event.addListener(distanceWidget, 'position_changed', function () {
            displayInfo(distanceWidget);
        });
    });
}
google.maps.event.addDomListener(window, 'load', initialize);



function DistanceWidget(place, map, position, placeName, address, infowindow) {
    this.set('map', map);
    this.set('position', position);
    if (marker && marker.setMap) {
        marker.setMap(null);
        marker = null;
    }

    marker = new google.maps.Marker({
        draggable: true,
        map: map
    });
    marker.setIcon(/** @type {google.maps.Icon} */({
        url: place.icon,
        size: new google.maps.Size(71, 71),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(17, 34),
        scaledSize: new google.maps.Size(35, 35)
    }));
    marker.setPosition(position)
    marker.setVisible(true);
    // Bind the marker map property to the DistanceWidget map property
    marker.bindTo('map', this);

    // Bind the marker position property to the DistanceWidget position
    // property
    marker.bindTo('position', this);
    infowindow.setContent('<div><strong>' + placeName + '</strong><br>' + address);
    infowindow.open(map, marker);

    // Create a new radius widget
    var radiusWidget = new RadiusWidget();

    // Bind the radiusWidget map to the DistanceWidget map
    radiusWidget.bindTo('map', this);

    // Bind the radiusWidget center to the DistanceWidget position
    radiusWidget.bindTo('center', this, 'position');
    // Bind to the radiusWidgets' distance property
    this.bindTo('distance', radiusWidget);

    // Bind to the radiusWidgets' bounds property
    this.bindTo('bounds', radiusWidget);
    //Need to set saved_widget to position or marker.setPosition(saved_widget('position'))
    //marker.setPosition(widget.get('position'));
    //infowindow.open(map, marker);
}
DistanceWidget.prototype = new google.maps.MVCObject();

function RadiusWidget() {
    var circle = new google.maps.Circle({
        strokeWeight: 2
    });

    // Set the distance property value, default to 50km.
    this.set('distance', 5);

    // Bind the RadiusWidget bounds property to the circle bounds property.
    this.bindTo('bounds', circle);

    // Bind the circle center to the RadiusWidget center property
    circle.bindTo('center', this);

    // Bind the circle map to the RadiusWidget map
    circle.bindTo('map', this);

    // Bind the circle radius property to the RadiusWidget radius property
    circle.bindTo('radius', this);
    this.addSizer_();
}
RadiusWidget.prototype = new google.maps.MVCObject();


/**
 * Update the radius when the distance has changed.
 */
RadiusWidget.prototype.distance_changed = function () {
    this.set('radius', this.get('distance') * 1000);
};
RadiusWidget.prototype.addSizer_ = function () {
    var sizer = new google.maps.Marker({
        draggable: true,
        title: 'Drag me!'
    });

    sizer.bindTo('map', this);
    sizer.bindTo('position', this, 'sizer_position');
    var me = this;
    google.maps.event.addListener(sizer, 'drag', function () {
        // Set the circle distance (radius)
        me.setDistance();
    });
};
RadiusWidget.prototype.center_changed = function () {
    var bounds = this.get('bounds');

    // Bounds might not always be set so check that it exists first.
    if (bounds) {
        var lng = bounds.getNorthEast().lng();

        // Put the sizer at center, right on the circle.
        var position = new google.maps.LatLng(this.get('center').lat(), lng);
        this.set('sizer_position', position);
    }
};

RadiusWidget.prototype.distanceBetweenPoints_ = function (p1, p2) {
    if (!p1 || !p2) {
        return 0;
    }

    var R = 6371; // Radius of the Earth in km
    var dLat = (p2.lat() - p1.lat()) * Math.PI / 180;
    var dLon = (p2.lng() - p1.lng()) * Math.PI / 180;
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(p1.lat() * Math.PI / 180) * Math.cos(p2.lat() * Math.PI / 180) *
      Math.sin(dLon / 2) * Math.sin(dLon / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d;
};

/**
 * Set the distance of the circle based on the position of the sizer.
 */
RadiusWidget.prototype.setDistance = function () {
    // As the sizer is being dragged, its position changes.  Because the
    // RadiusWidget's sizer_position is bound to the sizer's position, it will
    // change as well.
    var pos = this.get('sizer_position');
    var center = this.get('center');
    var distance = this.distanceBetweenPoints_(center, pos);

    // Set the distance property for any objects that are bound to it
    this.set('distance', distance);
};
function displayInfo(widget) {
    saved_widget = widget
    info.innerHTML = 'Position: ' + widget.get('position') + ', distance: ' +
  widget.get('distance');
    infowindow.close();
};

function codeLatLng(latlng, fn) {
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[1]) {
                    fn([
              (results[1].address_components[0] && results[1].address_components[0].short_name || ''),
              (results[1].address_components[1] && results[1].address_components[1].short_name || ''),
              (results[1].address_components[2] && results[1].address_components[2].short_name || '')
                    ].join(' '));
                } else {
                    alert('No results found');
                }
            } else {
                alert('Geocoder failed due to: ' + status);
            }
        });
    }
