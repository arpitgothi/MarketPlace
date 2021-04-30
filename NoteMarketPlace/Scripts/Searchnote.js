var type, category, university, course, country, rating;

    function snf(obj) {

    var data = {};

    if (obj.id == "noteType") {
        type = obj.value;
        }
    if (obj.id == "noteCategory") {
        category = obj.value;
        }
    if (obj.id == "university") {
        university = obj.value;
        }
    if (obj.id == "course") {
        course = obj.value;
        }
    if (obj.id == "country") {
        country = obj.value;
        }
    if (obj.id == "rating") {
        rating = obj.value;
        }

    data.Type = type;
    data.Category = category;
    data.University = university;
    data.Course = course;
    data.Country = country;
    data.Rating = rating;

    console.log(data);
    $.ajax({
        method: 'GET',
        url: 'SearchNote',
        data: data,
        success: function (data) {
            document.body.innerHTML = data;
            $('#status').fadeOut();
            $('#preloader').delay(350).fadeOut('slow');
        },
        error: function () {
            alert("Please Try Again");
        }
    });
}

$('#Search').on('keypress', function (e) {
    if (e.which == 13) {
        location.href = "Search_Notes?search=" + $(this).val();
    }
});


$('.details>h4').click(function () {
    location.href = "Note_Details/" + $(this).attr('noteId');
});